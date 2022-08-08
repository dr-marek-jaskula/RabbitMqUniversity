using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

var factory = new ConnectionFactory() { HostName = "localhost" };

using var connection = factory.CreateConnection();

using var channel = connection.CreateModel();

//Reply queue
var replyQueue = channel.QueueDeclare("", exclusive: true);
//Request queue
channel.QueueDeclare("request-queue", exclusive: false);

var consumer = new EventingBasicConsumer(channel);

consumer.Received += (model, ea) =>
{
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine($"Reply Received: {message}");
};

//consuming the reply
channel.BasicConsume(queue: replyQueue.QueueName, autoAck: true, consumer: consumer);

//Publishing the request

//1. We create a channel basic properties
var properties = channel.CreateBasicProperties();
//2. We add a "ReplyTo" property set to our reply queue name
properties.ReplyTo = replyQueue.QueueName;
//3. We use approach of correlation_id for both: CorrelationId for the reply should be guid
properties.CorrelationId = Guid.NewGuid().ToString();

var message = "Can I request a reply?";
var body = Encoding.UTF8.GetBytes(message);

Console.WriteLine($"Sending Request: {properties.CorrelationId}");
//We publish to the default exchange (however we should use a better exchange in the real world) 
//We pass a properties
channel.BasicPublish("", "request-queue", properties, body);

Console.ReadKey();