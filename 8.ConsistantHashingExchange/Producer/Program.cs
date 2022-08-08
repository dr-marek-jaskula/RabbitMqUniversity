using System.Text;
using RabbitMQ.Client;

var factory = new ConnectionFactory() { HostName = "localhost" };

using var connection = factory.CreateConnection();

using var channel = connection.CreateModel();

//As a type we need to type a string "x-consistent-hash" 
channel.ExchangeDeclare(exchange: "samplehashing", "x-consistent-hash");

var message = "Hello hash the routing key and pass me on please!";

//This will be hashed (its hash will determine where the message will be send)
var routingKeyToHash = "hash me!";

var body = Encoding.UTF8.GetBytes(message);

channel.BasicPublish("samplehashing", routingKeyToHash, null, body);

Console.WriteLine($"Send message: {message}");