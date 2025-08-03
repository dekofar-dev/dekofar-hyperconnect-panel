using System;
using System.Threading.Tasks;
using Dekofar.API.Authorization;
using Dekofar.API.Hubs;
using Dekofar.HyperConnect.Application.Common.Interfaces;
using Dekofar.HyperConnect.Application.UserMessages.Commands;
using Dekofar.HyperConnect.Application.UserMessages.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Dekofar.API.Controllers
{
    [ApiController]
    [Route("api/usermessages")]
    [Authorize]
    public class UserMessagesController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IFileStorageService _fileStorageService;
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly IApplicationDbContext _context;

        public UserMessagesController(IMediator mediator, IFileStorageService fileStorageService, IHubContext<ChatHub> hubContext, IApplicationDbContext context)
        {
            _mediator = mediator;
            _fileStorageService = fileStorageService;
            _hubContext = hubContext;
            _context = context;
        }

        [HttpGet("chat/{userId}")]
        public async Task<IActionResult> GetChat(Guid userId)
        {
            var messages = await _mediator.Send(new GetChatMessagesQuery(userId));
            return Ok(messages);
        }

        [HttpPost("upload")]
        [RequestSizeLimit(20_000_000)]
        public async Task<IActionResult> UploadMessageFile([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("File is empty");

            var userId = User.GetUserId();
            if (userId == null) return Unauthorized();

            var fileUrl = await _fileStorageService.SaveChatAttachmentAsync(file, userId.Value);
            return Ok(new { fileUrl, fileType = file.ContentType, fileSize = file.Length });
        }

        [HttpPost("send")]
        public async Task<IActionResult> Send([FromBody] SendUserMessageCommand command)
        {
            var message = await _mediator.Send(command);

            // Broadcast a lightweight notification to the receiver via SignalR.
            // The full message is persisted through MediatR command above.
            var preview = message.Text ?? string.Empty;
            if (preview.Length > 50)
                preview = preview.Substring(0, 50);

            await _hubContext.Clients.User(message.ReceiverId.ToString())
                .SendAsync("ReceiveMessage", new
                {
                    senderUserId = message.SenderId,
                    messagePreview = preview,
                    timestamp = message.SentAt
                });

            // Update unread message count badge for the receiver
            var unreadCount = await _context.UserMessages
                .CountAsync(m => m.ReceiverId == message.ReceiverId && !m.IsRead);

            await _hubContext.Clients.User(message.ReceiverId.ToString())
                .SendAsync("ReceiveUnreadCount", unreadCount);

            return Ok(message);
        }

        [HttpGet("unread-count")]
        public async Task<IActionResult> GetUnreadCount()
        {
            var count = await _mediator.Send(new GetUnreadMessageCountQuery());
            return Ok(count);
        }

        [HttpPut("mark-as-read/{chatUserId}")]
        public async Task<IActionResult> MarkAsRead(Guid chatUserId)
        {
            var updated = await _mediator.Send(new MarkMessagesAsReadCommand(chatUserId));
            var currentUserId = User.GetUserId();
            if (currentUserId != null)
            {
                // Notify the other participant that their messages were viewed
                await _hubContext.Clients.User(chatUserId.ToString())
                    .SendAsync("NotifyRead", new { readerId = currentUserId });

                // Refresh unread message counter for the current user
                var unreadCount = await _context.UserMessages
                    .CountAsync(m => m.ReceiverId == currentUserId && !m.IsRead);

                await _hubContext.Clients.User(currentUserId.Value.ToString())
                    .SendAsync("ReceiveUnreadCount", unreadCount);
            }

            return Ok(updated);
        }
    }
}
