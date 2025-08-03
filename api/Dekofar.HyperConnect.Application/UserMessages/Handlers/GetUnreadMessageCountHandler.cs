using System.Threading;
using System.Threading.Tasks;
using Dekofar.HyperConnect.Application.Common.Interfaces;
using Dekofar.HyperConnect.Application.UserMessages.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Dekofar.HyperConnect.Application.UserMessages.Handlers
{
    public class GetUnreadMessageCountHandler : IRequestHandler<GetUnreadMessageCountQuery, int>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public GetUnreadMessageCountHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<int> Handle(GetUnreadMessageCountQuery request, CancellationToken cancellationToken)
        {
            if (_currentUserService.UserId == null)
                throw new UnauthorizedAccessException();

            var currentUserId = _currentUserService.UserId.Value;

            return await _context.UserMessages
                .AsNoTracking()
                .CountAsync(m => m.ReceiverId == currentUserId && !m.IsRead, cancellationToken);
        }
    }
}
