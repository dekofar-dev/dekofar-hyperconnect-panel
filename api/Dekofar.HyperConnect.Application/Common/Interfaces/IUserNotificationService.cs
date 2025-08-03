using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dekofar.HyperConnect.Domain.Entities;

namespace Dekofar.HyperConnect.Application.Common.Interfaces
{
    public interface IUserNotificationService
    {
        Task<UserNotification> CreateAsync(Guid userId, string title, string message, string type);
        Task<List<UserNotification>> GetLatestAsync(Guid userId, int take = 20);
        Task MarkAsReadAsync(Guid userId, Guid notificationId);
        Task DeleteAsync(Guid userId, Guid notificationId);
    }
}
