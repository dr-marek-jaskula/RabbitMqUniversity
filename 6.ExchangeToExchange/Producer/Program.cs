using System.Text;
using RabbitMQ.Client;

var factory = new ConnectionFactory() { HostName = "localhost" };

using var connection = factory.CreateConnection();

using var channel = connection.CreateModel();

//We declare multiple exchanges and change them
channel.ExchangeDeclare(exchange: "firstexchange", type: ExchangeType.Direct);
channel.ExchangeDeclare(exchange: "secondexchange", type: ExchangeType.Fanout);

//We chain the exchange: first is is destination exchange, and second parameter is the source exchange, the third parameter is the routing key
channel.ExchangeBind("secondexchange", "firstexchange", "");

var message = "This message has gone through multiple exchanges";

var body = Encoding.UTF8.GetBytes(message);

//We publish is to the first exchange
channel.BasicPublish("firstexchange", "", null, body);

Console.WriteLine($"Send message: {message}");