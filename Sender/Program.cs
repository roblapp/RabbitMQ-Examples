namespace Sender
{
    using System;
    using System.Text;
    using RabbitMQ.Client;

    /// <summary>
    /// Sends Messages
    /// </summary>
    class Program
    {
        public static void Main()
        {
            var isRunning = true;
            var sender = new Sender();

            Console.WriteLine("Sender - Press q to exit.");
            
            while (isRunning)
            {
                var message = Console.ReadLine();
                if (message == "q") isRunning = false;
                else sender.SendToQueue(message);
            }

            Console.WriteLine("done");
        }
    }

    class Sender
    {
        public void SendToQueue(string message)
        {
            Console.WriteLine("Entered SendToQueue");
            var connectionFactory = new ConnectionFactory { HostName = "localhost" };

            using (var connection = connectionFactory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: "task_queue",
                                         durable: true,
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null);

                    var body = Encoding.UTF8.GetBytes(message);

                    var properties = channel.CreateBasicProperties();
                    properties.Persistent = true;

                    channel.BasicPublish(exchange: "",
                                         routingKey: "task_queue",
                                         basicProperties: properties,
                                         body: body);
                    Console.WriteLine("Sent {0}", message);
                }
            }

            Console.WriteLine("Exited SendToQueue");
        }
    }
}
