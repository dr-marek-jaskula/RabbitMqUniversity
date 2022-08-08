using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

var factory = new ConnectionFactory() { HostName = "localhost" };

using var connection = factory.CreateConnection();

using var channel = connection.CreateModel();

channel.ExchangeDeclare(exchange: "headersexchange", type: ExchangeType.Headers);

channel.QueueDeclare("letterbox");

//1. We create a binding arguments of type dictionary of string-object
var bindingArguments = new Dictionary<string, object>
{
    //we always need to set 'x-match' key, with value 'all' or 'any'
    //'any' - the message needs to have any of the specified headers
    //'all' - the message needs to have all specified header (except of "x-match" of course)
    { "x-match", "any" }, 
    { "name", "brian" },
    { "age", "21" }
};

channel.QueueBind("letterbox", "headersexchange", "", bindingArguments);

var consumer = new EventingBasicConsumer(channel);

consumer.Received += (model, eventArgs) =>
{
    var body = eventArgs.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine($"Recieved new message: {message}");
};

channel.BasicConsume(queue: "letterbox", autoAck: true, consumer: consumer);

Console.WriteLine("Consuming");

Console.ReadKey();