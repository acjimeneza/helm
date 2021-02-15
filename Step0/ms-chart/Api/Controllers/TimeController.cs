using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ms_chart.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class TimeController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        private readonly ILogger<TimeController> _logger;

        public TimeController(ILogger<TimeController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var rng = new Random();
            var result = await Task.FromResult<TimeDto>( new TimeDto
            {
                Date = DateTime.Now,
                Number = rng.Next(0, 55)
            });
            return Ok(result);
        }

        [HttpGet]
        [Route("conf")]
        public async Task<IActionResult>  GetConfig([FromQuery(Name = "conf")] string conf)
        {
            var rng = new Random();
            var result = await Task.FromResult<string>(_configuration[conf]);
            return Ok(result);
        }
    }
}
