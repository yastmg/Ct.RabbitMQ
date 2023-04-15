using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using System.Threading;
using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Speeding.Worker.Services
{
    public class SpeedingServices : BackgroundService
    {
        private readonly System.Timers.Timer timer = new System.Timers.Timer();

        public SpeedingServices()
        {
            timer.Interval = TimeSpan.FromMinutes(1).TotalMilliseconds;
            timer.Elapsed += new System.Timers.ElapsedEventHandler(LaunchRabbitMQ);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            timer.Start();
            return Task.CompletedTask;
        }

        void LaunchRabbitMQ(object sender, System.Timers.ElapsedEventArgs args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            var queueName = "Speeding";

            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += (sender, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                Console.WriteLine("Mensaje recibido: {0}", message);
            };

            channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);

            Console.WriteLine("Speeding En espera de mensajes...");
        }
    }
}
