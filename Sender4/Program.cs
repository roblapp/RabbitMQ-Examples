namespace Sender4
{
    using System;
    using System.Text;
    using RabbitMQ.Client;

    /// <summary>
    /// Sends Messages.
    /// </summary>
    class Program
    {
        public static void Main(string[] args)
        {
            var isRunning = true;
            var sender = new Sender4();

            Console.WriteLine("Sender - Press q to exit.");

            while (isRunning)
            {
                var line = Console.ReadLine();
                if (line == "q") isRunning = false;
                else
                {
                    if (line == null) throw new Exception("line is null");
                    var parsed = line.Split('$');
                    sender.SendToQueue(parsed[0], parsed[1]);
                }
            }

            Console.WriteLine("done");
        }
    }

    class Sender4
    {
        private const string Exchange = "topic_logs";

        public void SendToQueue(string message, string severity)
        {
            Console.WriteLine("Entered SendToQueue");

            var connectionFactory = new ConnectionFactory { HostName = "localhost" };

            using (var connection = connectionFactory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare(exchange: Exchange, type: "topic");

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
