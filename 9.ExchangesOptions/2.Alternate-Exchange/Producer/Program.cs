using System.Text;
using RabbitMQ.Client;

var factory = new ConnectionFactory() { HostName = "localhost" };

using var connection = factory.CreateConnection();

using var channel = connection.CreateModel();

channel.ExchangeDeclare(
    exchange: "altexchange",
    type: ExchangeType.Fanout); //usually the fanout exchange

channel.ExchangeDeclare(
    exchange: "mainexchange",
    type: ExchangeType.Direct,
    arguments: new Dictionary<string, object>
    {
        {"alternate-exchange", "altexchange"} //key needs to be "alternate-exchange" and value is a name of an exchange
    });

var message = "This is my first Message";

var body = Encoding.UTF8.GetBytes(message);

//change the routing key to go to the Alternate Exchange
channel.BasicPublish("mainexchange", "test", null, body);

Console.WriteLine($"Send message: {message}");
