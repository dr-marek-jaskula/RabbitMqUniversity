using System.Text;
using RabbitMQ.Client;

//Producer does not declare queue. 
//Producer does even know how many queues will get producer message
//Producer will just publish the message to the exchange. Exchange will do the further job.

var factory = new ConnectionFactory() { HostName = "localhost" };

using var connection = factory.CreateConnection();

using var channel = connection.CreateModel();

//We declare the exchange explicitly of type "Fanout"
channel.ExchangeDeclare(exchange: "pubsub", type: ExchangeType.Fanout);

var message = "Hello I want to broadcast this message";

var body = Encoding.UTF8.GetBytes(message);

//We punlish it into publish exchange.
channel.BasicPublish(exchange: "pubsub", "", null, body);

Console.WriteLine($"Send message: {message}");