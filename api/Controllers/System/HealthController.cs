using Dekofar.HyperConnect.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Dekofar.API.Controllers.System
{
    [ApiController]
    [Route("api/system/health-check")]
    [Authorize(Roles = "Admin")]
    public class HealthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public HealthController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Check()
        {
            var dbStatus = await _context.Database.CanConnectAsync() ? "OK" : "FAILED";
            var response = new
            {
                database = dbStatus,
                fileStorage = "UNKNOWN",
                netgsm = "UNKNOWN",
                smsBalance = 0,
                uptime = Environment.TickCount64 / 1000 + " seconds"
            };
            return Ok(response);
        }
    }
}
