using System;
using System.Linq;
using System.Threading.Tasks;
using Dekofar.HyperConnect.Application.Common.Interfaces;
using Dekofar.HyperConnect.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Dekofar.HyperConnect.Infrastructure.Services
{
    public class BadgeService : IBadgeService
    {
        private readonly IApplicationDbContext _context;
        private readonly IUserNotificationService _notificationService;

        public BadgeService(IApplicationDbContext context, IUserNotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

        public async Task EvaluateAsync(Guid userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return;

            if (user.MembershipDate <= DateTime.UtcNow.AddMonths(-6))
            {
                bool has = await _context.UserBadges.AnyAsync(b => b.UserId == userId && b.Badge == "Joined 6+ months ago");
                if (!has)
                {
                    var badge = new UserBadge
                    {
                        Id = Guid.NewGuid(),
                        UserId = userId,
                        Badge = "Joined 6+ months ago",
                        AwardedAt = DateTime.UtcNow
                    };
                    await _context.UserBadges.AddAsync(badge);
                    await _context.SaveChangesAsync();
                    await _notificationService.CreateAsync(userId, "Badge Earned", "Joined 6+ months ago", "badge");
                }
            }
        }
    }
}
