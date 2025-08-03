using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dekofar.HyperConnect.Application.Common.Interfaces;
using Dekofar.HyperConnect.Application.UserMessages.DTOs;
using Dekofar.HyperConnect.Application.UserMessages.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Dekofar.HyperConnect.Application.UserMessages.Handlers
{
    public class GetChatMessagesHandler : IRequestHandler<GetChatMessagesQuery, List<UserMessageDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public GetChatMessagesHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<List<UserMessageDto>> Handle(GetChatMessagesQuery request, CancellationToken cancellationToken)
        {
            if (_currentUserService.UserId == null)
                throw new UnauthorizedAccessException();

            var currentUserId = _currentUserService.UserId.Value;

            return await _context.UserMessages
                .AsNoTracking()
                .Where(m => (m.SenderId == currentUserId && m.ReceiverId == request.ChatUserId) ||
                            (m.SenderId == request.ChatUserId && m.ReceiverId == currentUserId))
                .OrderBy(m => m.SentAt)
                .Select(m => new UserMessageDto
                {
                    Id = m.Id,
                    SenderId = m.SenderId,
                    ReceiverId = m.ReceiverId,
                    Text = m.Text,
                    FileUrl = m.FileUrl,
                    FileType = m.FileType,
                    FileSize = m.FileSize,
                    SentAt = m.SentAt,
                    IsRead = m.IsRead,
                    ReadAt = m.ReadAt
                })
                .ToListAsync(cancellationToken);
        }
    }
}
