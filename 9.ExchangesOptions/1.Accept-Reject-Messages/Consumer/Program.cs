using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

var factory = new ConnectionFactory() { HostName = "localhost" };

using var connection = factory.CreateConnection();

using var channel = connection.CreateModel();

channel.ExchangeDeclare(
    exchange: "acceptrejectexchange",
    type: ExchangeType.Fanout);

channel.QueueDeclare(queue: "letterbox");
channel.QueueBind("letterbox", "acceptrejectexchange", ""); //they will be rejected because the routing key is not "test"

var consumer = new EventingBasicConsumer(channel);
consumer.Received += (model, eventArgs) =>
{
    var body = eventArgs.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);

    //We manually acknowledge the message. 
    //The delivery tag is unique
    if (eventArgs.DeliveryTag % 5 is 0)
    {
        //we acknowledge multiple messages at once, when the delivery tag is equal to 5, 10, 15,...
        channel.BasicAck(deliveryTag: eventArgs.DeliveryTag, multiple: true);

        //We can reject multiple messages (we do not want requeue them, we could but it could make a loop - if we have multiple consumer the loop should not be present)
        //channel.BasicNack(deliveryTag: eventArgs.DeliveryTag, requeue: false, multiple: true);
    }

    //channel.BasicReject(deliveryTag: eventArgs.DeliveryTag, requeue: false);

    Console.WriteLine($"Main - Received new message: {message}");
};
channel.BasicConsume(queue: "letterbox", consumer: consumer);

Console.WriteLine("Consuming");

Console.ReadKey();