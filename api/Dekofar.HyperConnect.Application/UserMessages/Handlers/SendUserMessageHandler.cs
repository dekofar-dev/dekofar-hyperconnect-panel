using System;
using System.Threading;
using System.Threading.Tasks;
using Dekofar.HyperConnect.Application.Common.Interfaces;
using Dekofar.HyperConnect.Application.Interfaces;
using Dekofar.HyperConnect.Application.UserMessages.Commands;
using Dekofar.HyperConnect.Application.UserMessages.DTOs;
using Dekofar.HyperConnect.Domain.Entities;
using MediatR;

namespace Dekofar.HyperConnect.Application.UserMessages.Handlers
{
    public class SendUserMessageHandler : IRequestHandler<SendUserMessageCommand, UserMessageDto>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        private readonly IModerationService _moderation;

        public SendUserMessageHandler(IApplicationDbContext context, ICurrentUserService currentUserService, IModerationService moderation)
        {
            _context = context;
            _currentUserService = currentUserService;
            _moderation = moderation;
        }

        public async Task<UserMessageDto> Handle(SendUserMessageCommand request, CancellationToken cancellationToken)
        {
            if (_currentUserService.UserId == null)
                throw new UnauthorizedAccessException();

            var check = await _moderation.CheckAsync(request.Text, _currentUserService.UserId);
            if (check.Blocked)
                throw new InvalidOperationException("Your message contains restricted content.");

            var message = new UserMessage
            {
                Id = Guid.NewGuid(),
                SenderId = _currentUserService.UserId.Value,
                ReceiverId = request.ReceiverId,
                Text = request.Text,
                FileUrl = request.FileUrl,
                FileType = request.FileType,
                FileSize = request.FileSize,
                SentAt = DateTime.UtcNow,
                IsRead = false
            };

            await _context.UserMessages.AddAsync(message, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return new UserMessageDto
            {
                Id = message.Id,
                SenderId = message.SenderId,
                ReceiverId = message.ReceiverId,
                Text = message.Text,
                FileUrl = message.FileUrl,
                FileType = message.FileType,
                FileSize = message.FileSize,
                SentAt = message.SentAt,
                IsRead = message.IsRead,
                ReadAt = message.ReadAt
            };
        }
    }
}
