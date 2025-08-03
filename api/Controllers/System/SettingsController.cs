using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Dekofar.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SettingsController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public SettingsController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [Authorize]
        [HttpGet("pin-timeout")]
        public IActionResult GetPinTimeout()
        {
            var minutes = _configuration.GetValue<int>("PinTimeoutInMinutes", 5);
            return Ok(new { timeout = minutes });
        }
    }
}
