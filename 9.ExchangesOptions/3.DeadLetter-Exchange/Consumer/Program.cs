using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

var factory = new ConnectionFactory() { HostName = "localhost" };

using var connection = factory.CreateConnection();

using var channel = connection.CreateModel();

channel.ExchangeDeclare(
    exchange: "mainexchange",
    type: ExchangeType.Direct);

channel.ExchangeDeclare(
    exchange: "dlx",
    type: ExchangeType.Fanout); //usually it is Fanout

//Now we need to configure the queue as the rejected or expired messages will go from queue to dead letter exchange
channel.QueueDeclare(
    queue: "mainexchangequeue",
    arguments: new Dictionary<string, object>
    {
        {"x-dead-letter-exchange", "dlx"}, //to define that the dead letter exchange is the exchange with name "dlx"
        {"x-message-ttl", 1000}, //time to expire. It is 1 second
    });

channel.QueueBind("mainexchangequeue", "mainexchange", "test");

var mainConsumer = new EventingBasicConsumer(channel);
mainConsumer.Received += (model, eventArgs) =>
{
    var body = eventArgs.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine($"Main - Received new message: {message}");
};

//We comment is out because we do not want the messages to be consumed at this moment (we want them to expire)
//channel.BasicConsume(queue: "mainexchangequeue", consumer: mainConsumer); //uncomment this to consume messages from main queue

//We create a second queue for a dlx exchange
channel.QueueDeclare(queue: "dlxexchangequeue");
channel.QueueBind("dlxexchangequeue", "dlx", "");

var dlxConsumer = new EventingBasicConsumer(channel);
dlxConsumer.Received += (model, ea) =>
{
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine($"DLX - Received new message: {message}");
};
channel.BasicConsume(queue: "dlxexchangequeue", consumer: dlxConsumer);

Console.WriteLine("Consuming");

Console.ReadKey();