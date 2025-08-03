using Dekofar.HyperConnect.Domain.Entities;
using Dekofar.HyperConnect.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using TaskStatus = Dekofar.HyperConnect.Domain.Entities.TaskStatus;

namespace Dekofar.API.Controllers.Admin
{
    [ApiController]
    [Route("api/calendar-tasks")]
    [Authorize(Roles = "Admin")]
    public class CalendarTasksController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public CalendarTasksController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<CalendarTask>> Create([FromBody] CalendarTask task)
        {
            _context.CalendarTasks.Add(task);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = task.Id }, task);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CalendarTask>>> Get([FromQuery] string? assignedTo)
        {
            var query = _context.CalendarTasks.AsQueryable();
            if (!string.IsNullOrEmpty(assignedTo))
                query = query.Where(t => t.AssignedUserId == assignedTo);
            var tasks = await query.ToListAsync();
            return Ok(tasks);
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] TaskStatus status)
        {
            var task = await _context.CalendarTasks.FindAsync(id);
            if (task == null) return NotFound();
            task.Status = status;
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
