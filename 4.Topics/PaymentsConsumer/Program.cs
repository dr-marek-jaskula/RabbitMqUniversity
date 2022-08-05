﻿using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

var factory = new ConnectionFactory() { HostName = "localhost" };

using var connection = factory.CreateConnection();

using var channel = connection.CreateModel();

channel.ExchangeDeclare(exchange: "topic", type: ExchangeType.Topic);

var queueName = channel.QueueDeclare().QueueName;

//and with .peyments
channel.QueueBind(queue: queueName, exchange: "topic", routingKey: "#.payments");

var consumer = new EventingBasicConsumer(channel);

consumer.Received += (model, ea) =>
{
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine($"Payments - Recieved new message: {message}");
};

channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);

Console.WriteLine("Payments Consuming");

Console.ReadKey();