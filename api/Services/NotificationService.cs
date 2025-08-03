using Dekofar.HyperConnect.Application.Common.Interfaces;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading;
using System.Threading.Tasks;
using Dekofar.API.Hubs;

namespace Dekofar.API.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationService(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public Task SendToUserAsync(Guid userId, string method, object? arg, CancellationToken cancellationToken = default)
        {
            return _hubContext.Clients.Group($"user-{userId}").SendAsync(method, arg, cancellationToken);
        }
    }
}
