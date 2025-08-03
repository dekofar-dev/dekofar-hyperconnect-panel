using Dekofar.HyperConnect.Application.Common.Interfaces;
using Dekofar.HyperConnect.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Dekofar.API.Controllers
{
    [ApiController]
    [Route("api/pin-covers")]
    public class PinCoversController : ControllerBase
    {
        private readonly IApplicationDbContext _context;

        public PinCoversController(IApplicationDbContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        [HttpGet("active")]
        public async Task<IActionResult> GetActive()
        {
            var now = DateTime.UtcNow.TimeOfDay;
            var images = await _context.PinCoverImages
                .Where(p => p.Active &&
                            (!p.DisplayStartTime.HasValue || !p.DisplayEndTime.HasValue ||
                             (p.DisplayStartTime <= p.DisplayEndTime
                                ? now >= p.DisplayStartTime && now <= p.DisplayEndTime
                                : now >= p.DisplayStartTime || now <= p.DisplayEndTime)))
                .ToListAsync();
            return Ok(images);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PinCoverImage image)
        {
            _context.PinCoverImages.Add(image);
            await _context.SaveChangesAsync();
            return Ok(image.Id);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _context.PinCoverImages.FindAsync(id);
            if (entity == null)
                return NotFound();
            _context.PinCoverImages.Remove(entity);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}/activate")]
        public async Task<IActionResult> Activate(int id, [FromBody] bool active)
        {
            var entity = await _context.PinCoverImages.FindAsync(id);
            if (entity == null)
                return NotFound();
            entity.Active = active;
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
