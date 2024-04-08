using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Snai.CMS.Api_Core.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly ILogger<HomeController > _logger;
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var value = "hello";
            _logger.LogInformation($"輸入的資料：{value}");
            return Ok(new { Id = 1, Value = $"You say {value}" });
        }
    }
}
