using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PanicButton.Services
{
    public class PanicButtonServices: BackgroundService
    {
        private readonly System.Timers.Timer timer = new System.Timers.Timer();

        public PanicButtonServices() {
            timer.Interval = TimeSpan.FromMinutes(1).TotalMilliseconds;
            timer.Elapsed += new System.Timers.ElapsedEventHandler(LaunchRabbitMQ);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            timer.Start();
            return Task.CompletedTask;
        }

        void LaunchRabbitMQ(object sender, System.Timers.ElapsedEventArgs args) {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            var queueName = "PB";

            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += (sender, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                Console.WriteLine("Mensaje recibido: {0}", message);
            };

            channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);

            Console.WriteLine("PB En espera de mensajes...");
        }
    }
}
