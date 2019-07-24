using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthController : ControllerBase
    {
        private readonly ILogger _logger;

        public HealthController(ILogger<HealthController> logger)
        {
            _logger = logger;
        }

        // GET: api/<controller>
        [HttpGet("status/")]
        public ActionResult<HealsStatus> Get()
        {
           
            _logger.LogInformation("Health check answering...");
         
            return Ok(new HealsStatus() {
                ServiceID = "0001",
                ServiceName = "Bridge",
                ServceInfo = "Bridge v 1.1.1"
            });
        }
     
    }
}
