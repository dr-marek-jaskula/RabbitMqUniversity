using System.Text;
using RabbitMQ.Client;

var factory = new ConnectionFactory() { HostName = "localhost" };

using var connection = factory.CreateConnection();

using var channel = connection.CreateModel();

channel.ExchangeDeclare(exchange: "headersexchange", type: ExchangeType.Headers);

var message = "This message will be sent with headers";

var body = Encoding.UTF8.GetBytes(message);

//1. We create properties
var properties = channel.CreateBasicProperties();

//2. We create headers as a dictionary of string-object pairs
properties.Headers = new Dictionary<string, object>
{
    { "name", "brian" },
};

//3. We publish it
channel.BasicPublish("headersexchange", "", properties, body);

Console.WriteLine($"Send message: {message}");