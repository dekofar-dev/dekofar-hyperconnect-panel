using Dekofar.HyperConnect.Domain.Entities;
using Dekofar.HyperConnect.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dekofar.API.Controllers.Admin
{
    [ApiController]
    [Route("api/blacklist")]
    [Authorize(Roles = "Admin")]
    public class BlacklistController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public BlacklistController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BlacklistEntry>>> GetAll()
        {
            var items = await _context.BlacklistEntries.ToListAsync();
            return Ok(items);
        }

        [HttpPost]
        public async Task<ActionResult<BlacklistEntry>> Create([FromBody] BlacklistEntry entry)
        {
            _context.BlacklistEntries.Add(entry);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetAll), new { id = entry.Id }, entry);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var entry = await _context.BlacklistEntries.FindAsync(id);
            if (entry == null) return NotFound();
            _context.BlacklistEntries.Remove(entry);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
