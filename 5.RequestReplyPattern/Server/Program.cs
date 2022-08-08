using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

var factory = new ConnectionFactory() { HostName = "localhost" };

using var connection = factory.CreateConnection();

using var channel = connection.CreateModel();

channel.QueueDeclare("request-queue", exclusive: false);

//We create a new consumer, because server receives and sends the messages
var consumer = new EventingBasicConsumer(channel);

consumer.Received += (model, eventArgs) =>
{
    Console.WriteLine($"Received Request: {eventArgs.BasicProperties.CorrelationId}");

    var replyMessage = $"This is your reply: {eventArgs.BasicProperties.CorrelationId}";

    var body = Encoding.UTF8.GetBytes(replyMessage);

    //We publish to the default exchange (here it is a popular approach)
    channel.BasicPublish("", eventArgs.BasicProperties.ReplyTo, null, body);
};

//For consuming the requests
channel.BasicConsume(queue: "request-queue", autoAck: true, consumer: consumer);

Console.ReadKey(); 