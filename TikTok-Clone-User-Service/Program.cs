
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
            
            builder.Services.AddDbContext<DbUserContext>();


            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddCors();

            //dbContext
            using (var client = new DbUserContext())
            {
                client.Database.EnsureCreated();
            }


            //add RabbitMQ
            builder.Services.AddSingleton<IRabbitMQService, RabbitMQService>();
            builder.Services.AddSingleton<RabbitMQVideoConsumer>();


            var app = builder.Build();

            app.UseCors(builder =>
            {
                builder.WithOrigins("http://localhost:3000", "*")
                .AllowAnyMethod()
                .AllowAnyHeader();
            });
            // Configure the HTTP request pipeline.
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


            app.Run();
            // Start the RabbitMQ consumer
            var rabbitMQConsumer = app.Services.GetRequiredService<RabbitMQVideoConsumer>();
            rabbitMQConsumer.ConsumeMessage("comment_exchange", "testing_comment_queue", "routing_key");
        }
    }
}
