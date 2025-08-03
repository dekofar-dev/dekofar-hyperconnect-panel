using System;
using System.Threading.Tasks;
using Dekofar.HyperConnect.Application.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dekofar.API.Controllers
{
    [ApiController]
    [Route("api/status")]
    public class StatusController : ControllerBase
    {
        private readonly IApplicationDbContext _context;

        public StatusController(IApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{userId:guid}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetStatus(Guid userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return NotFound();
            return Ok(new { isOnline = user.IsOnline, lastSeen = user.LastSeen });
        }
    }
}
