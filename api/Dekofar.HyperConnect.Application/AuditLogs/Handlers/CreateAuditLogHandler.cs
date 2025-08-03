using Dekofar.HyperConnect.Application.AuditLogs.Commands;
using Dekofar.HyperConnect.Application.Common.Interfaces;
using Dekofar.HyperConnect.Domain.Entities;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Dekofar.HyperConnect.Application.AuditLogs.Handlers
{
    public class CreateAuditLogHandler : IRequestHandler<CreateAuditLogCommand, Unit>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;

        public CreateAuditLogHandler(IApplicationDbContext context, ICurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        public async Task<Unit> Handle(CreateAuditLogCommand request, CancellationToken cancellationToken)
        {
            if (_currentUser.UserId == null)
                throw new UnauthorizedAccessException();

            var log = new AuditLog
            {
                Id = Guid.NewGuid(),
                Action = request.Action,
                TargetType = request.TargetType,
                TargetId = request.TargetId,
                Description = request.Description,
                PerformedByUserId = _currentUser.UserId.Value,
                Timestamp = DateTime.UtcNow
            };

            await _context.AuditLogs.AddAsync(log, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}
