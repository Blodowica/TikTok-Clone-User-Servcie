using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
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

            // Database configuration
            var connectionString = builder.Configuration.GetConnectionString("userDatabase");
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

            app.UseSwagger();
            app.UseSwaggerUI();

            if (!app.Environment.IsDevelopment())
            {
                app.UseHttpsRedirection();
            }

            app.UseAuthorization();
            app.MapControllers();

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
                    Password = rabbitMQConfig["Password"],
                    VirtualHost = rabbitMQConfig["Virtualhost"]
                };

                // Register RabbitMQ services
               
                builder.Services.AddSingleton(connectionFactory);
                builder.Services.AddHostedService(provider =>
                    new RabbitMQVideoConsumer(connectionFactory, provider.GetRequiredService<IServiceProvider>(), "video_exchange", "like_video_queue"));
            }
            catch (Exception ex)
            {
                // Handle RabbitMQ configuration exception
                Console.WriteLine($"Failed to configure RabbitMQ: {ex.Message}");
                throw;
            }
        }
    }
}
