namespace Receiver4
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
        private const string Exchange = "topic_logs";

        public static void Main(string[] args)
        {
            Console.WriteLine(string.Join(", ", args));

            var factory = new ConnectionFactory { HostName = "localhost" };

            //Receiver
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare(exchange: Exchange, type: "topic");

                    var queueName = channel.QueueDeclare().QueueName;

                    foreach (var item in args)
                    {
                        channel.QueueBind(queue: queueName, exchange: Exchange, routingKey: item);
                    }

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
