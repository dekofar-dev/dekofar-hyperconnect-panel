using System;
using System.Linq;
using System.Threading.Tasks;
using Dekofar.HyperConnect.Application.Common.Interfaces;
using Dekofar.HyperConnect.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Dekofar.HyperConnect.Infrastructure.Jobs
{
    public class SupportTicketJobService
    {
        private const int StaleTicketDays = 30;
        private readonly IApplicationDbContext _context;
        private readonly ILogger<SupportTicketJobService> _logger;

        public SupportTicketJobService(IApplicationDbContext context, ILogger<SupportTicketJobService> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Closes support tickets that have been open without updates for more than <see cref="StaleTicketDays"/> days.
        /// </summary>
        public async Task CloseOldTickets()
        {
            var threshold = DateTime.UtcNow.AddDays(-StaleTicketDays);

            var staleTickets = await _context.SupportTickets
                .Where(t => t.Status == SupportTicketStatus.Open && t.LastUpdatedAt < threshold)
                .ToListAsync();

            if (staleTickets.Count == 0)
            {
                _logger.LogInformation("No stale support tickets found to close.");
                return;
            }

            foreach (var ticket in staleTickets)
            {
                ticket.Status = SupportTicketStatus.Closed;
            }

            await _context.SaveChangesAsync();
            _logger.LogInformation("Closed {Count} stale support tickets.", staleTickets.Count);
        }

        /// <summary>
        /// Sends a summary notification about unassigned tickets older than 24 hours.
        /// </summary>
        public async Task NotifyAdminOfUnassignedTickets()
        {
            var threshold = DateTime.UtcNow.AddHours(-24);

            var unassignedTickets = await _context.SupportTickets
                .Where(t => t.AssignedUserId == null && t.CreatedAt < threshold)
                .ToListAsync();

            if (unassignedTickets.Count == 0)
            {
                _logger.LogInformation("No unassigned support tickets pending notification.");
                return;
            }

            // In a real implementation an email or message would be sent to administrators.
            _logger.LogWarning("{Count} support tickets remain unassigned for over 24 hours.", unassignedTickets.Count);
        }
    }
}

