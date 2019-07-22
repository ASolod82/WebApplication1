using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthController : ControllerBase
    {
        // GET: api/<controller>
        [HttpGet("status/")]
        public HealsStatus  Get()
        {

            return new HealsStatus() {
                ServiceID = "0001",
                ServiceName = "Bridge",
                ServceInfo = "Bridge v 1.1.1"
            };
        }
     
    }
}
