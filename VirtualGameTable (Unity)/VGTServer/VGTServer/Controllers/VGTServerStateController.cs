using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VGTServer.Answers;

namespace VGTServer.Controllers
{
    [Route("/api/state/")]
    [ApiController]
    public class VGTServerStateController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetServerState()
        {
            return Ok("Active");
        }
    }
}
