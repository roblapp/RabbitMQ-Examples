namespace Sender3
{
    using System;
    using System.Text;
    using RabbitMQ.Client;

    /// <summary>
    /// Sends Messages
    /// </summary>
    class Program
    {
        public static void Main(string[] args)
        {
            var isRunning = true;
            var sender = new Sender3();
            var severity = args[0];

            Console.WriteLine("Sender - Press q to exit.");

            while (isRunning)
            {
                var message = Console.ReadLine();
                if (message == "q") isRunning = false;
                else sender.SendToQueue(message, severity);
            }

            Console.WriteLine("done");
        }
    }

    class Sender3
    {
        private const string Exchange = "direct_logs";

        public void SendToQueue(string message, string severity)
        {
            Console.WriteLine("Entered SendToQueue");

            var connectionFactory = new ConnectionFactory { HostName = "localhost" };

            using (var connection = connectionFactory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    //channel.QueueDeclare(queue: "task_queue",
                    //                     durable: true,
                    //                     exclusive: false,
                    //                     autoDelete: false,
                    //                     arguments: null);
                    channel.ExchangeDeclare(exchange: Exchange, type: "direct");

                    var body = Encoding.UTF8.GetBytes(message);

                    channel.BasicPublish(exchange: Exchange,
                                         routingKey: severity,
                                         basicProperties: null,
                                         body: body);

                    Console.WriteLine("Sent {0}", message);
                }
            }

            Console.WriteLine("Exited SendToQueue");
        }
    }
}
