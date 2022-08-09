using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

var factory = new ConnectionFactory() { HostName = "localhost" };

using var connection = factory.CreateConnection();

using var channel = connection.CreateModel();

channel.ExchangeDeclare(
    exchange: "altexchange",
    type: ExchangeType.Fanout);

channel.ExchangeDeclare(
    exchange: "mainexchange",
    type: ExchangeType.Direct,
    arguments: new Dictionary<string, object>
    {
        {"alternate-exchange", "altexchange"}
    });

//alternate exchange
channel.QueueDeclare(queue: "altexchangequeue");
channel.QueueBind("altexchangequeue", "altexchange", "");

//consuming from alternate exchange
var altConsumer = new EventingBasicConsumer(channel);
altConsumer.Received += (model, ea) =>
{
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine($"Alt - Received new message: {message}");
};
channel.BasicConsume(queue: "altexchangequeue", consumer: altConsumer);

//main exchange
channel.QueueDeclare(queue: "mainexchangequeue");
channel.QueueBind("mainexchangequeue", "mainexchange", "test");

var mainConsumer = new EventingBasicConsumer(channel);
mainConsumer.Received += (model, eventArgs) =>
{
    var body = eventArgs.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine($"Main - Received new message: {message}");
};
channel.BasicConsume(queue: "mainexchangequeue", consumer: mainConsumer);

Console.WriteLine("Consuming");

Console.ReadKey();
