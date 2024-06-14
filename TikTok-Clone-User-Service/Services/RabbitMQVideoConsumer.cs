using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using TikTok_Clone_User_Service.Models;

namespace TikTok_Clone_User_Service.Services
{
    public class RabbitMQVideoConsumer
    {
        private readonly ConnectionFactory _connectionFactory;
        private readonly IServiceProvider _serviceProvider;

        public RabbitMQVideoConsumer(ConnectionFactory connectionFactory, IServiceProvider serviceProvider)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public void ConsumeMessage(string exchangeName, string queueName, string routingKey)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var channel = connection.CreateModel();

            // Declare the exchange if it doesn't exist with the desired properties
            channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Direct, durable: true, autoDelete: false, arguments: null);

            // Declare the queue to ensure the queue exists
            channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

            channel.QueueBind(queue: queueName,
                              exchange: exchangeName,
                              routingKey: routingKey);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                Console.WriteLine("Received: {0}", message);

                // Handle the received message here
                HandleMessage(message, queueName);
            };

            channel.BasicConsume(queue: queueName,
                                 autoAck: false,
                                 consumer: consumer);

            Console.WriteLine($"Consuming messages from queue '{queueName}'...");
            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }

        private void HandleMessage(string message, string queueName)
        {
            // Implement your message handling logic here
            Console.WriteLine($"Handling message: {message}");
            // Example: Store message in database, process it, etc.
            if (queueName == "like_video_queue")
            {
                var likeAction = JsonConvert.DeserializeObject<LikeActionDTO>(message);

                if (likeAction != null && likeAction.Status == "liked")
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var likeActionService = scope.ServiceProvider.GetRequiredService<ILikeActionService>();
                        try
                        {
                            likeActionService.addUserLikedVideos(likeAction.AuthId, likeAction.VideoId);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error processing like action: {ex.Message}");
                        }
                    }
                }
                else if (likeAction != null && likeAction.Status == "disliked")
                {
                    // Implement dislike handling if necessary
                }
            }
        }
    }
}
