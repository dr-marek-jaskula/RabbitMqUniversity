using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

var factory = new ConnectionFactory() { HostName = "localhost" };

using var connection = factory.CreateConnection();

using var channel = connection.CreateModel();

//1. Exchange declaration 
channel.ExchangeDeclare(exchange: "samplehashing", "x-consistent-hash");

//2. We declare multiple queues
var queue1 = channel.QueueDeclare("letterbox1");
var queue2 = channel.QueueDeclare("letterbox2");

//2. We bind to them. As a binding keys we set numeric values as a strings
channel.QueueBind("letterbox1", "samplehashing", "1"); //this will take 25% messages. The hash space is here smaller 
channel.QueueBind("letterbox2", "samplehashing", "3"); //this will take 75% messages. The hash space is here bigger (depending on the hash key)

var consumer1 = new EventingBasicConsumer(channel);
consumer1.Received += (model, eventArgs) =>
{
    var body = eventArgs.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine($"Queue1 - Received new message: {message}");
};

var consumer2 = new EventingBasicConsumer(channel);
consumer2.Received += (model, ea) =>
{
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine($"Queue2 - Received new message: {message}");
};

//We set consuming
channel.BasicConsume(queue: "letterbox1", autoAck: true, consumer: consumer1);
channel.BasicConsume(queue: "letterbox2", autoAck: true, consumer: consumer2);

Console.WriteLine("Consuming");

Console.ReadKey();