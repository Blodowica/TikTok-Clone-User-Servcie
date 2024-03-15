using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TikTok_Clone_User_Service.Services;

namespace TikTok_Clone_User_Service.Controllers
{
    [Route("api/sendMessage")]
    [ApiController]
    public class RabbitMQTestController : ControllerBase
    {
        private readonly IRabbitMQService _rabbitMQService;
        private readonly ILogger<RabbitMQTestController> _logger;

        public RabbitMQTestController(IRabbitMQService rabbitMQService, ILogger<RabbitMQTestController> logger)
        {
            _rabbitMQService = rabbitMQService;
            _logger = logger;
        }

        [HttpPost]
        public IActionResult Post( string message)
        {
            _rabbitMQService.SendMessage(message);
            return Ok("Message sent to RabbitMQ.");
        }
    }
}
