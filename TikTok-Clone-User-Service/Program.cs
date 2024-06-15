using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using System;
using TikTok_Clone_User_Service.DatabaseContext;
using TikTok_Clone_User_Service.Services;

namespace TikTok_Clone_User_Service
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddScoped<IRabbitMQService, RabbitMQService>();
            builder.Services.AddScoped<ILikeActionService, LikeActionService>();

            //databse configuration
            var connectionString = builder.Configuration.GetConnectionString("userDatabase");
            Console.WriteLine(connectionString);
            builder.Services.AddDbContext<DbUserContext>(options =>
                options.UseSqlServer(connectionString));

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddCors();

            // Configuration for RabbitMQ
            ConfigureRabbitMQ(builder);

            var app = builder.Build();

            // Middleware and pipeline configuration
            app.UseCors(builder =>
            {
                builder.WithOrigins("http://localhost:3000", "*")
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            });

            if (app.Environment.IsDevelopment())
            {
            }
            app.UseSwagger();
            app.UseSwaggerUI();

            if (!app.Environment.IsDevelopment())
            {
                app.UseHttpsRedirection();
            }

            app.UseAuthorization();
            app.MapControllers();

            // Start the RabbitMQ consumer for different queues
            StartRabbitMQConsumers(app);

            app.Run();
        }

        private static void ConfigureRabbitMQ(WebApplicationBuilder builder)
        {
            try
            {
                var rabbitMQConfig = builder.Configuration.GetSection("RabbitMQConfiguration");
                var connectionFactory = new ConnectionFactory
                {
                    HostName = rabbitMQConfig["Hostname"],
                    Port = Convert.ToInt32(rabbitMQConfig["Port"]),
                    UserName = rabbitMQConfig["Username"],
                    Password = rabbitMQConfig["Password"]
                };

                // Register RabbitMQ services
                builder.Services.AddSingleton(connectionFactory);
                builder.Services.AddSingleton<RabbitMQVideoConsumer>();
            }
            catch (Exception ex)
            {
                // Handle RabbitMQ configuration exception
                Console.WriteLine($"Failed to configure RabbitMQ: {ex.Message}");
                throw;
            }
        }

        private static void StartRabbitMQConsumers(WebApplication app)
        {
            try
            {
                // Get a scope to resolve scoped services
                using var scope = app.Services.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<DbUserContext>();

                // Check if the database is available
                if (!dbContext.Database.CanConnect())
                {
                    throw new Exception("Database is not available");
                }

                var rabbitMQConsumer = app.Services.GetRequiredService<RabbitMQVideoConsumer>();

                // Example: Start consumers for different queues
                rabbitMQConsumer.ConsumeMessage("video_exchange", "like_video_queue", "like_video_queue");
                //rabbitMQConsumer.ConsumeMessage("exchange_name", "queue_name_2", "routing_key_2");
                // Add more consumers as needed with different queue names
            }
            catch (Exception ex)
            {
                // Handle RabbitMQ consumer start exception
                Console.WriteLine($"Failed to start RabbitMQ consumers: {ex.Message}");
                throw;
            }
        }
    }
}
