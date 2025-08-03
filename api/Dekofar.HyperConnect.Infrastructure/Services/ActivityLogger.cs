using System;
using System.Text.Json;
using System.Threading.Tasks;
using Dekofar.HyperConnect.Application.Common.Interfaces;
using Dekofar.HyperConnect.Domain.Entities;

namespace Dekofar.HyperConnect.Infrastructure.Services
{
    public class ActivityLogger : IActivityLogger
    {
        private readonly IApplicationDbContext _context;

        public ActivityLogger(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task LogAsync(Guid userId, string actionType, object? data, string? ipAddress)
        {
            var log = new ActivityLog
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                ActionType = actionType,
                ActionData = data != null ? JsonSerializer.Serialize(data) : null,
                IpAddress = ipAddress,
                CreatedAt = DateTime.UtcNow
            };

            await _context.ActivityLogs.AddAsync(log);
            await _context.SaveChangesAsync();
        }
    }
}
