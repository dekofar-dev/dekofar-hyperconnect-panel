using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Dekofar.API.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        /// <summary>
        /// Thread safe store mapping user IDs to active SignalR connection IDs.
        /// This allows targeting a specific user when sending messages.
        /// </summary>
        public static readonly ConcurrentDictionary<string, string> Connections = new();

        /// <summary>
        /// Called whenever a client connects to the hub.
        /// The authenticated user's identifier (<see cref="HubCallerContext.UserIdentifier"/>)
        /// is mapped to the current connection id so that we can later address
        /// that user directly.
        /// </summary>
        public override Task OnConnectedAsync()
        {
            var userId = Context.UserIdentifier;
            if (!string.IsNullOrEmpty(userId))
            {
                Connections[userId] = Context.ConnectionId;
            }
            return base.OnConnectedAsync();
        }

        /// <summary>
        /// Cleans up the connection mapping when a client disconnects for any reason.
        /// </summary>
        public override Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.UserIdentifier;
            if (!string.IsNullOrEmpty(userId))
            {
                Connections.TryRemove(userId, out _);
            }
            return base.OnDisconnectedAsync(exception);
        }

        #region Chat Methods
        /// <summary>
        /// Sends a plain text message to the specified user.
        /// The receiver is identified by their user identifier which is set as the
        /// <c>UserIdentifier</c> claim during authentication.
        /// </summary>
        public Task SendMessage(string receiverUserId, string message)
        {
            return Clients.User(receiverUserId)
                .SendAsync("ReceiveMessage", Context.UserIdentifier, message);
        }
        #endregion
    }
}
