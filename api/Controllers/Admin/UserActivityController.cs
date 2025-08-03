using System;
using System.Linq;
using System.Threading.Tasks;
using Dekofar.HyperConnect.Application.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dekofar.API.Controllers.Admin
{
    [ApiController]
    [Route("api/admin")]
    [Authorize(Roles = "Admin")]
    public class UserActivityController : ControllerBase
    {
        private readonly IApplicationDbContext _context;

        public UserActivityController(IApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("user-activity-summary")]
        public async Task<IActionResult> GetUserActivitySummary()
        {
            var loginStats = await _context.ActivityLogs
                .Where(a => a.ActionType == "Login")
                .GroupBy(a => a.UserId)
                .Select(g => new { UserId = g.Key, LoginCount = g.Count(), LastActivity = g.Max(a => a.CreatedAt) })
                .ToListAsync();

            var users = await _context.Users
                .Select(u => new { u.Id, u.FullName })
                .ToListAsync();

            var result = users.Select(u =>
            {
                var stat = loginStats.FirstOrDefault(s => s.UserId == u.Id);
                return new
                {
                    u.Id,
                    u.FullName,
                    LoginCount = stat?.LoginCount ?? 0,
                    LastActivity = stat?.LastActivity
                };
            })
            .OrderByDescending(r => r.LoginCount)
            .ToList();

            return Ok(result);
        }

        [HttpGet("activity-logs")]
        public async Task<IActionResult> GetLogs([FromQuery] Guid? userId, [FromQuery] string? actionType, [FromQuery] DateTime? from, [FromQuery] DateTime? to)
        {
            var query = _context.ActivityLogs.AsQueryable();
            if (userId.HasValue)
                query = query.Where(l => l.UserId == userId.Value);
            if (!string.IsNullOrEmpty(actionType))
                query = query.Where(l => l.ActionType == actionType);
            if (from.HasValue)
                query = query.Where(l => l.CreatedAt >= from.Value);
            if (to.HasValue)
                query = query.Where(l => l.CreatedAt <= to.Value);

            var logs = await query.OrderByDescending(l => l.CreatedAt).Take(200).ToListAsync();
            return Ok(logs);
        }
    }
}
