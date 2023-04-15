using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text;
using RabbitMQ.Client;
using Producer.Domain;

namespace Producer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HubController : ControllerBase
    {
        [HttpPost]
        [Route("CrearProducto")]
        public IActionResult CrearProducto([FromBody] RegisterGps registerGps)
        {
            // ...

            var factory = new ConnectionFactory() { HostName = "localhost" }; // o la dirección del servidor de RabbitMQ
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare("registerGps", ExchangeType.Topic, durable: true, autoDelete: false);

                var queueName = registerGps.typeEvent;
                channel.QueueDeclare(queueName, durable: true, exclusive: false, autoDelete: false);
                channel.QueueBind(queueName, "registerGps", routingKey: queueName);

                var message = JsonSerializer.Serialize(registerGps);
                var body = Encoding.UTF8.GetBytes(message);

                var properties = channel.CreateBasicProperties();
                properties.Persistent = true;

                channel.BasicPublish("registerGps", queueName, properties, body);
            }

            // ...

            return Ok(registerGps);
        }
    }
}
