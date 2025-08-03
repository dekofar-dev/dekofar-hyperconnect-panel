using Dekofar.HyperConnect.Domain.Entities;
using Dekofar.HyperConnect.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Dekofar.API.Controllers.Admin
{
    [ApiController]
    [Route("api/deployments")]
    [Authorize(Roles = "Admin")]
    public class DeploymentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public DeploymentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<DeploymentLog>> Create([FromBody] DeploymentLog log)
        {
            _context.DeploymentLogs.Add(log);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetAll), new { id = log.Id }, log);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var logs = await _context.DeploymentLogs
                .OrderByDescending(l => l.DeployedAt)
                .ToListAsync();
            return Ok(logs);
        }

        [HttpGet("latest")]
        public async Task<IActionResult> GetLatest()
        {
            var log = await _context.DeploymentLogs
                .OrderByDescending(l => l.DeployedAt)
                .FirstOrDefaultAsync();
            if (log == null) return NotFound();
            return Ok(log);
        }
    }
}
