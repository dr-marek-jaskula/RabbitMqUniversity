using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

var factory = new ConnectionFactory() { HostName = "localhost" };

using var connection = factory.CreateConnection();

using var channel = connection.CreateModel();

//We publish to explicitly declared exchange. Therefore, if the produces would be called first, it need to be present
//Parameter "exchange" with value "routing" is name of the exchange, type is a type, here Direct (we can use the binding keys)
channel.ExchangeDeclare(exchange: "routing", type: ExchangeType.Direct);

//Declare the queue with a random name
var queueName = channel.QueueDeclare().QueueName;

//Here we will bind queues to exchange using explicitly defined binding keys
channel.QueueBind(queue: queueName, exchange: "routing", routingKey: "analyticsonly");
//We create also the second binding with routing key for both of them
channel.QueueBind(queue: queueName, exchange: "routing", routingKey: "both");

var consumer = new EventingBasicConsumer(channel);

consumer.Received += (model, ea) =>
{
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine($"Analytics - Received new message: {message}");
};

//We need to specify the queue name
channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);

Console.WriteLine("Analytics Consuming");

Console.ReadKey();