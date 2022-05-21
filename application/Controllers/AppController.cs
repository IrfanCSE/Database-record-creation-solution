using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace application.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AppController : ControllerBase
    {

        [HttpGet]
        [Route("Configuration")]
        public async Task<IActionResult> Get()
        {
            return Ok("I Developed a .Net Core library & application that that uses the library that will insert / update records into a database.");
        }
    }
}
