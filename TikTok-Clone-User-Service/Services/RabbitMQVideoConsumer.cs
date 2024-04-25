using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace TikTok_Clone_User_Service.Services
{
    public class RabbitMQVideoConsumer
    {
        private readonly ConnectionFactory _connectionFactory;

        public RabbitMQVideoConsumer(IConfiguration configuration)
        {
            var rabbitMQConfig = configuration.GetSection("RabbitMQConfiguration");

            _connectionFactory = new ConnectionFactory
            {
                HostName = rabbitMQConfig["Hostname"],
                Port = Convert.ToInt32(rabbitMQConfig["Port"]),
                UserName = rabbitMQConfig["Username"],
                Password = rabbitMQConfig["Password"]
            };
        }

        public void ConsumeMessage(string exchangeName, string queueName, string routingKey)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Direct);

            channel.QueueDeclare(queue: queueName,
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            channel.QueueBind(queue: queueName,
                              exchange: exchangeName,
                              routingKey: routingKey);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine("Received: {0}", message);

                // TODO: Add your logic to handle the message
            };
            channel.BasicConsume(queue: queueName,
                                 autoAck: true,
                                 consumer: consumer);

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }
    }
}
