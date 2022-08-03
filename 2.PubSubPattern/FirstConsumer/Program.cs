using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

var factory = new ConnectionFactory() { HostName = "localhost" };

using var connection = factory.CreateConnection();

using var channel = connection.CreateModel();

//We also declare the exchange here because consumer can start before producer (we need to ensure that exchange exists. If it this will do nothing so it is safe)
channel.ExchangeDeclare(exchange: "pubsub", type: ExchangeType.Fanout);

//We will create temporary queues. They will just exist as long as consumer exists. When consumer will be turned off, the queue will be deleted.
//In the same line we also get the QueueName that will be auto generated (because it will just live the same time as consumer, there is not need for explicit name)
var queueName = channel.QueueDeclare().QueueName;

//We bind the consumer to the queue, and queue to the exchange (this is core line)
channel.QueueBind(queue: queueName, exchange: "pubsub", routingKey: "");

var consumer = new EventingBasicConsumer(channel);

consumer.Received += (model, ea) =>
{
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine($"FirstConsumer - Received new message: {message}");
};

//for a simple example we will leave autoAct as true
channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);

Console.WriteLine("Consuming");

Console.ReadKey();