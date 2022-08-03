using RabbitMQ.Client;
using System.Text;

//0. Install RabbitMQ.Client NuGet Package

//1. Create the connection factory
var factory = new ConnectionFactory() { HostName = "localhost" };

//2. create a connection (and dispose it when it is no longer needed)
using var connection = factory.CreateConnection();

//3. create a channel (and dispose it when it is no longer needed)
using var channel = connection.CreateModel();

//4. we declare a queue that is connected to the channel. 
channel.QueueDeclare(
    queue: "letterbox", 
    durable: false, 
    exclusive: false, 
    autoDelete: false, 
    arguments: null);

var random = new Random();

int messageId = 1;

//Process multiple messages (we will have many consumers)
while (true)
{
    //publishing time is faster then processing time
    var publishingTime = random.Next(1, 4);

    //5. Write a message
    var message = $"Sending message with MessageId: {messageId}";

    //6. Encode the message into the stream of bytes
    var encodedMessange = Encoding.UTF8.GetBytes(message);

    //7. Here the exchange is the default one so we just use "".
    channel.BasicPublish("", "letterbox", null, encodedMessange);

    Console.WriteLine($"Published message: {message}");

    Task.Delay(TimeSpan.FromSeconds(publishingTime)).Wait();

    messageId++;
}


