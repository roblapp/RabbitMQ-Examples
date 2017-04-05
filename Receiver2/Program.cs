namespace Receiver2
{
    using System;
    using System.Text;
    using System.Threading;
    using RabbitMQ.Client;
    using RabbitMQ.Client.Events;

    /// <summary>
    /// Receives messages
    /// </summary>
    class Program
    {
        public static void Main(string[] args)
        {
            var factory = new ConnectionFactory { HostName = "localhost" };

            //Receiver
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    //channel.QueueDeclare(queue: "task_queue",
                    //    durable: true,
                    //    exclusive: false,
                    //    autoDelete: false,
                    //    arguments: null);
                    channel.ExchangeDeclare(exchange: "logs", type: "fanout");

                    var queueName = channel.QueueDeclare().QueueName;

                    channel.QueueBind(queue: queueName, exchange: "logs", routingKey: "");

                    //TODO experiment with this
                    ////This tells RabbitMQ not to give more than one message to a worker at a time.
                    ////Or, in other words, don't dispatch a new message to a worker until it has
                    ////processed and acknowledged the previous one. Instead, it will dispatch it to
                    ////the next worker that is not still busy.
                    //channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

                    Console.WriteLine("...Waiting for messages...");

                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body;
                        var message = Encoding.UTF8.GetString(body);
                        Console.WriteLine("---------------------------------------");
                        Console.WriteLine("Received {0}", message);

                        Console.WriteLine("model: " + model.ToString());
                        Console.WriteLine("exchange: " + ea.Exchange);
                        Console.WriteLine("routing key: " + ea.RoutingKey);
                        Console.WriteLine("consumer tag: " + ea.ConsumerTag);
                        Console.WriteLine("delivery tag: " + ea.DeliveryTag);
                        Console.WriteLine("redelivered: " + ea.Redelivered);


                        //Fake some work...
                        int dots = message.Split('.').Length;
                        if (dots > 0)
                        {
                            Thread.Sleep(1000 * dots);
                        }

                        Console.WriteLine("Completed processing message '{0}'", message);
                        Console.WriteLine("---------------------------------------");
                        Console.WriteLine();
                        Console.WriteLine();
                        //channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                    };

                    channel.BasicConsume(queue: queueName,
                        noAck: true,
                        consumer: consumer);

                    Console.WriteLine(" Press [enter] to exit.");
                    Console.ReadLine();
                }
            }
        }
    }
}
