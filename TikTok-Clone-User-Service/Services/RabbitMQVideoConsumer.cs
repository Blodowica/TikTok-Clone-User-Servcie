using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TikTok_Clone_User_Service.Models;

namespace TikTok_Clone_User_Service.Services
{
    public class RabbitMQVideoConsumer : BackgroundService
    {
        private readonly ConnectionFactory _connectionFactory;
        private readonly IServiceProvider _serviceProvider;
        private IModel _channel;

        public RabbitMQVideoConsumer(ConnectionFactory connectionFactory, IServiceProvider serviceProvider)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var connection = _connectionFactory.CreateConnection();
            _channel = connection.CreateModel();

            // Declare the exchange if it doesn't exist with the desired properties
            _channel.ExchangeDeclare(exchange: "video_exchange", type: ExchangeType.Direct, durable: true, autoDelete: false, arguments: null);

            // Declare the queue to ensure the queue exists
            _channel.QueueDeclare(queue: "like_video_queue", durable: true, exclusive: false, autoDelete: false, arguments: null);

            _channel.QueueBind(queue: "like_video_queue", exchange: "video_exchange", routingKey: "like_video_queue");

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                Console.WriteLine("Received: {0}", message);

                // Handle the received message here
                await HandleMessageAsync(message, "like_video_queue");
            };

            _channel.BasicConsume(queue: "like_video_queue", autoAck: true, consumer: consumer);

            Console.WriteLine($"Consuming messages from queue 'like_video_queue'...");
            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            _channel?.Close();
            _channel?.Dispose();
            base.Dispose();
        }

        private async Task HandleMessageAsync(string message, string queueName)
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
                            await likeActionService.addUserLikedVideos(likeAction.AuthId, likeAction.VideoId);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error processing like action: {ex.Message}");
                        }
                    }
                }
                else if (likeAction != null && likeAction.Status == "disliked")
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var likeActionService = scope.ServiceProvider.GetRequiredService<ILikeActionService>();
                        try
                        {
                           await likeActionService.removeUserLikedVideos(likeAction.AuthId, likeAction.VideoId);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error processing like action: {ex.Message}");
                        }
                    }
                }
            }
        }
    }
}
