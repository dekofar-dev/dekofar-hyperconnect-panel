using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Dekofar.HyperConnect.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Dekofar.API.Controllers
{
    [ApiController]
    [Route("api/account")]
    [Authorize]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly PasswordHasher<ApplicationUser> _passwordHasher = new();
        private readonly IConfiguration _configuration;

        public AccountController(UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        [HttpPost("set-pin")]
        public async Task<IActionResult> SetPin([FromBody] PinDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return Unauthorized();

            user.HashedPin = _passwordHasher.HashPassword(user, dto.Pin);
            user.PinLastUpdatedAt = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);
            return Ok();
        }

        [HttpPost("verify-pin")]
        public async Task<IActionResult> VerifyPin([FromBody] PinDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || string.IsNullOrEmpty(user.HashedPin)) return Unauthorized();

            var result = _passwordHasher.VerifyHashedPassword(user, user.HashedPin, dto.Pin);
            if (result == PasswordVerificationResult.Success)
            {
                return Ok();
            }

            return Unauthorized();
        }

        [HttpGet("pin-timeout")]
        [AllowAnonymous]
        public IActionResult GetPinTimeout()
        {
            var timeout = _configuration.GetValue<int>("PinLock:TimeoutInMinutes");
            return Ok(new { timeoutInMinutes = timeout });
        }
    }
}
