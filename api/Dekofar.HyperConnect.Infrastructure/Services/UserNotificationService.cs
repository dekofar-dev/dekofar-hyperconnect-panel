using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dekofar.HyperConnect.Application.Common.Interfaces;
using Dekofar.HyperConnect.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Dekofar.HyperConnect.Infrastructure.Services
{
    public class UserNotificationService : IUserNotificationService
    {
        private readonly IApplicationDbContext _context;
        private readonly INotificationService _notificationService;

        public UserNotificationService(IApplicationDbContext context, INotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

        public async Task<UserNotification> CreateAsync(Guid userId, string title, string message, string type)
        {
            var notification = new UserNotification
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Title = title,
                Message = message,
                Type = type,
                CreatedAt = DateTime.UtcNow
            };

            await _context.UserNotifications.AddAsync(notification);
            await _context.SaveChangesAsync();

            await _notificationService.SendToUserAsync(userId, "ReceiveNotification", new { notification.Id, notification.Title, notification.Message, notification.Type, notification.CreatedAt });

            return notification;
        }

        public async Task<List<UserNotification>> GetLatestAsync(Guid userId, int take = 20)
        {
            return await _context.UserNotifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .Take(take)
                .ToListAsync();
        }

        public async Task MarkAsReadAsync(Guid userId, Guid notificationId)
        {
            var notif = await _context.UserNotifications.FirstOrDefaultAsync(n => n.Id == notificationId && n.UserId == userId);
            if (notif != null)
            {
                notif.IsRead = true;
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(Guid userId, Guid notificationId)
        {
            var notif = await _context.UserNotifications.FirstOrDefaultAsync(n => n.Id == notificationId && n.UserId == userId);
            if (notif != null)
            {
                _context.UserNotifications.Remove(notif);
                await _context.SaveChangesAsync();
            }
        }
    }
}
