# RabbitMq

RabbitMq is a open-source message broker that enables services and applications to communicate with each other. 
It supports multiple messaging protocols like AMQP (Advanced Message Queuing Protocol), and MQTT (MQ Telemetry Transport). STOMP (Simple (or Streaming) Text Orientated Messaging Protocol)

## How it works

RabbitMQ **exchange** receives **messages** from **producers** and pushes them to **queues** depending on the rules defined by the RabbitMQ exchange type. 

1. Producers are publishers - systems or clients that are publishing the messages. 
2. Queues are containers for messages and they are processed in **first-in-first-out** way. 
3. Exchange is a **routing mediator** and distributes the messages based on a **routing keys**, **bindings** and **header attributes**.
4. Binding is a connection between a queue and an exchange
5. Routing Key is a message attribute taken into account by the exchange when deciding how to route a message.

Message flow:

1. Producer publishes the message
2. Message is sent to the exchange that producer specified
3. Exchange determine to which queues pass the message 
4. Queues store message until consumers can process them
5. Consumers obtain messages from queues

## Exchanges

Producer never delivers a message straight to a message queue, but to **exchange**. 

Types:
1. Direct exchange
2. Topic exchange
3. Fanout exchange
4. Headers exchange

Additional types:
1. Default exchange
2. DeadLetter exchange
3. Alternate exchange

## Direct exchange

This exchange type uses a message routing key to transport messages to queue. 
The routing key is a message attribute that the producer adds to the message header.
You can consider the routing key to be an “address” that the exchange uses to determine how the message should be routed.
A message is delivered to the queue with the binding key that exactly matches the message’s routing key. 

The direct exchange default exchange is “amq. direct“, which AMQP brokers must offer for communication.

## Topic exchange

This exchange type sends messages to queues depending on **wildcard** matches between the **routing key** and the queue **binding’s routing pattern**
Messages are routed to one or more queues based on a pattern that matches a message routing key.
A list of words separated by a period must be used as the routing key.

The routing patterns may include an asterisk \* to match a word in a specified position of the routing key.
For instance a routing pattern of “hello.*.*.b.*” matches routing keys: 
1. hello.mine.super.b.message
2. hello.extra.mega.b.ignore

A hash symbol (“#”) denotes a match of zero or more words. For instance for "hello.*.to.#"
1. hello.lazy.to.hiper.extra.super
2. hello.lazy.to.you

## Fanout exchange

This exchange routes a received message to any associated queues, regardless of routing keys or pattern matching. Provided keys will be entirely ignored. 

Fanout exchanges are useful when the same message needs to be passed to one or perhaps more queues with consumers who may process the message differently.

## Headers exchange

This exchange sends messages to queues depending on **arguments with headers** and **optional values**. Header exchanges are identical to topic exchanges, except that instead of using routing keys, messages are routed based on header values.
If the value of the header equals the value of supply during binding, the message matches.

In the binding between exchange and queue, a specific argument termed “x-match” indicates whether all headers must match or only one. 
The “x-match” property has two possible values: “any” and “all,” with “all” being the default.
A value of “all” indicates that all header pairs (key, value) must match, whereas “any” indicates that at least one pair must match.
Instead of a string, headers can be built with a larger range of data types, such as integers or hashes. 

## Default exchange

The default exchange is an unnamed pre-declared direct exchange. 
Usually, an empty string is frequently used to indicate it.
If you choose default exchange, your message will be delivered to a queue with the same name as the routing key.
With a routing key that is the same as the queue name, every queue is immediately tied to the default exchange.

## Dead Letter Exchange

A message is dropped if there is no matching queue for it.
Dead Letter Exchanges must be implemented so that those messages can be saved and reprocessed later.
The “Dead Letter Exchange” is an AMQP enhancement provided by RabbitMQ. This exchange has the capability of capturing messages that are not deliverable.

## Useful commands

> rabbitmqctl status

> rabbitmq-plugins --help

> rabbitmq-plugins list