using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace Dekofar.API.Hubs
{
    [Authorize]
    public class SupportHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            var userId = Context.User?.FindFirst("id")?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"user-{userId}");
            }

            if (Context.User?.IsInRole("Admin") == true || Context.User?.IsInRole("Support") == true)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, "SupportAgents");
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.User?.FindFirst("id")?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"user-{userId}");
            }

            if (Context.User?.IsInRole("Admin") == true || Context.User?.IsInRole("Support") == true)
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, "SupportAgents");
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}
