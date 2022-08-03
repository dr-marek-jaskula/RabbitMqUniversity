using RabbitMQ.Client;
using RabbitMQ.Client.Events;
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

//Basic quality service. These are parameters for Competing Consumer Pattern:
//It limits the number of unacknowledged messages on a channel (or connection) when consuming (aka "prefetch count"). 
//prefetchCount = 1 -> This tells RabbitMQ not to give more than one message to a worker at a time. Or, in other words, don't dispatch a new message to a worker until it has processed and acknowledged the previous one. Instead, it will dispatch it to the next worker that is not still busy.
//prefetchSize is the parameter to control unacknowledged messages size. A value of 0 is treated as infinite.
//The server will send a message in advance if it is equal to or smaller in size than the available prefetch size (and also falls into other prefetch limits). May be set to zero, meaning "no specific limit", although other prefetch limits may still apply. The prefetch-size is ignored if the no-ack option is set. 
//The server MUST ignore this setting when the client is not processing any messages - i.e. the prefetch size does not limit the transfer of single messages to a client, only the sending in advance of more messages while the client still has one or more unacknowledged messages. 

//global: false -> shared across all consumers on the channel (applied separately to each new consumer on the channel)
//global: true -> shared across all consumers on the connection (shared across all consumers on the channel)
channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false); //This is the key to make task dispatched to workers that are ready to work

//5. Create an eventing consumer
var consumer = new EventingBasicConsumer(channel);

//random for a random processing time
var random = new Random();

//6. Add to the received event the delegate that will write our message to the console
consumer.Received += (model, eventArgs) =>
{
    var processingTime = random.Next(1, 6);

    var body = eventArgs.Body.ToArray();

    var message = Encoding.UTF8.GetString(body);

    Console.WriteLine($"Message Received: {message} will take {processingTime} to process");

    Task.Delay(TimeSpan.FromSeconds(processingTime)).Wait();

    //Delivery Tags determines which message we were delivering
    //This is manual Acknowledge (Ack from Acknowledge) 
    channel.BasicAck(deliveryTag: eventArgs.DeliveryTag, multiple: false);
};

//7. Bind the consumer with a channel
//autoAct = true means that every message is automatically acknowledged (as soon as we get it).
//autoAct = false means we need to manually acknowledged messages
//autoAct = false is better. We implement the Competing Consumer Pattern (this gives us opportunity to say the task is done)
channel.BasicConsume(queue: "letterbox", autoAck: false, consumer: consumer);

Console.ReadKey();
