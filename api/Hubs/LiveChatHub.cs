using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Dekofar.HyperConnect.Application.Common.Interfaces;
using Dekofar.HyperConnect.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Dekofar.API.Hubs
{
    [Authorize]
    public class LiveChatHub : Hub
    {
        public static readonly ConcurrentDictionary<string, string> ConnectedUsers = new();
        private readonly IApplicationDbContext _context;

        public LiveChatHub(IApplicationDbContext context)
        {
            _context = context;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Context.UserIdentifier;
            if (!string.IsNullOrEmpty(userId))
            {
                ConnectedUsers[userId] = Context.ConnectionId;

                if (Guid.TryParse(userId, out var guid))
                {
                    var user = await _context.Users.FindAsync(guid);
                    if (user != null)
                    {
                        user.IsOnline = true;
                        await _context.SaveChangesAsync();
                    }
                }
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.UserIdentifier;
            if (!string.IsNullOrEmpty(userId))
            {
                ConnectedUsers.TryRemove(userId, out _);

                if (Guid.TryParse(userId, out var guid))
                {
                    var user = await _context.Users.FindAsync(guid);
                    if (user != null)
                    {
                        user.IsOnline = false;
                        user.LastSeen = DateTime.UtcNow;
                        await _context.SaveChangesAsync();
                    }
                }
            }

            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(string receiverId, string text, string? fileUrl = null, string? fileType = null, long? fileSize = null)
        {
            var senderId = Context.UserIdentifier;
            if (string.IsNullOrEmpty(senderId) || string.IsNullOrEmpty(receiverId))
                return;

            if (!Guid.TryParse(senderId, out var senderGuid) || !Guid.TryParse(receiverId, out var receiverGuid))
                return;

            var message = new UserMessage
            {
                Id = Guid.NewGuid(),
                SenderId = senderGuid,
                ReceiverId = receiverGuid,
                Text = text,
                FileUrl = fileUrl,
                FileType = fileType,
                FileSize = fileSize,
                SentAt = DateTime.UtcNow,
                IsRead = false
            };

            await _context.UserMessages.AddAsync(message);

            var sender = await _context.Users.FindAsync(senderGuid);
            if (sender != null)
            {
                sender.LastMessageDate = message.SentAt;
            }

            var receiver = await _context.Users.FindAsync(receiverGuid);
            if (receiver != null)
            {
                receiver.LastMessageDate = message.SentAt;
            }

            await _context.SaveChangesAsync();

            if (ConnectedUsers.TryGetValue(receiverId, out var connectionId))
            {
                await Clients.Client(connectionId).SendAsync("ReceiveMessage", new
                {
                    senderId,
                    receiverId,
                    text,
                    fileUrl,
                    fileType,
                    fileSize,
                    sentAt = message.SentAt
                });
            }
        }

        public async Task NotifyRead(string receiverId, Guid readerId)
        {
            if (ConnectedUsers.TryGetValue(receiverId, out var connectionId))
            {
                await Clients.Client(connectionId).SendAsync("NotifyRead", new { readerId });
            }
        }

        public async Task ReportUploadProgress(string receiverId, int progress)
        {
            if (ConnectedUsers.TryGetValue(receiverId, out var connectionId))
            {
                await Clients.Client(connectionId).SendAsync("UploadProgress", progress);
            }
        }
    }
}

