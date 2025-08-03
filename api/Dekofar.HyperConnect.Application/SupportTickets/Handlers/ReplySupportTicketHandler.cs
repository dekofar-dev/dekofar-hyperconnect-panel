using Dekofar.HyperConnect.Application.Common.Interfaces;
using Dekofar.HyperConnect.Application.SupportTickets.Commands;
using Dekofar.HyperConnect.Application.SupportTickets.DTOs;
using Dekofar.HyperConnect.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Dekofar.HyperConnect.Application.SupportTickets.Handlers
{
    public class ReplySupportTicketHandler : IRequestHandler<ReplySupportTicketCommand, SupportTicketReplyDto>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;
        private readonly IHostEnvironment _env;

        public ReplySupportTicketHandler(IApplicationDbContext context, ICurrentUserService currentUser, IHostEnvironment env)
        {
            _context = context;
            _currentUser = currentUser;
            _env = env;
        }

        public async Task<SupportTicketReplyDto> Handle(ReplySupportTicketCommand request, CancellationToken cancellationToken)
        {
            if (_currentUser.UserId == null)
                throw new UnauthorizedAccessException();

            var ticket = await _context.SupportTickets.FindAsync(new object?[] { request.TicketId }, cancellationToken);
            if (ticket == null)
                throw new Exception("Ticket not found");

            var reply = new SupportTicketReply
            {
                Id = Guid.NewGuid(),
                TicketId = ticket.Id,
                UserId = _currentUser.UserId.Value,
                Message = request.Message,
                CreatedAt = DateTime.UtcNow
            };

            if (request.File != null)
            {
                var uploads = Path.Combine(_env.ContentRootPath, "uploads");
                Directory.CreateDirectory(uploads);
                var fileName = Guid.NewGuid() + Path.GetExtension(request.File.FileName);
                var fullPath = Path.Combine(uploads, fileName);
                using var stream = new FileStream(fullPath, FileMode.Create);
                await request.File.CopyToAsync(stream, cancellationToken);
                reply.AttachmentUrl = fullPath;
            }

            _context.SupportTicketReplies.Add(reply);
            ticket.LastUpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);

            return new SupportTicketReplyDto
            {
                Id = reply.Id,
                TicketId = reply.TicketId,
                SenderId = reply.UserId,
                TicketOwnerId = ticket.CreatedByUserId,
                Message = reply.Message,
                AttachmentUrl = reply.AttachmentUrl,
                CreatedAt = reply.CreatedAt
            };
        }
    }
}
