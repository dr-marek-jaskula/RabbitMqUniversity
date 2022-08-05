using System.Text;
using RabbitMQ.Client;

var factory = new ConnectionFactory() { HostName = "localhost" };

using var connection = factory.CreateConnection();

using var channel = connection.CreateModel();

//Declare exchange (producer can be first)
channel.ExchangeDeclare(exchange: "routing", type: ExchangeType.Direct);

//We do not need to declare queues

var message = "This message needs to be routed";

var body = Encoding.UTF8.GetBytes(message);

//We publish with routing key
//channel.BasicPublish(exchange: "routing", routingKey: "analyticsonly", null, body);
//channel.BasicPublish(exchange: "routing", routingKey: "paymentsonly", null, body);
channel.BasicPublish(exchange: "routing", routingKey: "both", null, body);

Console.WriteLine($"Send message: {message}");