using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dekofar.HyperConnect.Application.Common.Interfaces;
using Dekofar.HyperConnect.Application.UserMessages.Commands;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Dekofar.HyperConnect.Application.UserMessages.Handlers
{
    public class MarkMessagesAsReadHandler : IRequestHandler<MarkMessagesAsReadCommand, int>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public MarkMessagesAsReadHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<int> Handle(MarkMessagesAsReadCommand request, CancellationToken cancellationToken)
        {
            if (_currentUserService.UserId == null)
                throw new UnauthorizedAccessException();

            var currentUserId = _currentUserService.UserId.Value;

            var messages = await _context.UserMessages
                .Where(m => m.SenderId == request.ChatUserId && m.ReceiverId == currentUserId && !m.IsRead)
                .ToListAsync(cancellationToken);

            foreach (var message in messages)
            {
                message.IsRead = true;
                message.ReadAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync(cancellationToken);

            return messages.Count;
        }
    }
}
