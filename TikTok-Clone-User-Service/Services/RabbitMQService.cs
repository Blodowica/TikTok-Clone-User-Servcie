using RabbitMQ.Client;
using System;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace TikTok_Clone_User_Service.Services
{
    public interface IRabbitMQService
    {
        void SendMessage(string message);
    }

    public class RabbitMQService : IRabbitMQService
    {
        private readonly ConnectionFactory _connectionFactory;
        private readonly string _queueName;

        public RabbitMQService(IConfiguration configuration)
        {
            // Use the correct IP address and port of your Docker container
            _connectionFactory = new ConnectionFactory
            {
                HostName = "172.17.0.2", // Docker container IP address
                Port = 5672,              // RabbitMQ default port
                UserName = "guest",
                Password = "guest"
            };
            _queueName = "UserPublishQueue";
        }

        public void SendMessage(string message)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(queue: _queueName,
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            var body = Encoding.UTF8.GetBytes(message);

            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;

            channel.BasicPublish(exchange: "",
                                 routingKey: _queueName,
                                 basicProperties: properties,
                                 body: body);

            Console.WriteLine($" [x] Sent {message}");
        }
    }
}
