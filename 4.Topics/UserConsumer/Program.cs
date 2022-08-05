using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

var factory = new ConnectionFactory() { HostName = "localhost" };

using var connection = factory.CreateConnection();

using var channel = connection.CreateModel();

channel.ExchangeDeclare(exchange: "topic", type: ExchangeType.Topic);

var queueName = channel.QueueDeclare().QueueName;

//We can use wildcards '*' (one world) and '#' (zero or more)
channel.QueueBind(queue: queueName, exchange: "topic", routingKey: "user.#");

var consumer = new EventingBasicConsumer(channel);

consumer.Received += (model, ea) =>
{
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine($"Users - Received new message: {message}");
};

channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);

Console.WriteLine("Users Consuming");

Console.ReadKey();