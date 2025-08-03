using Dekofar.HyperConnect.Application.Common.Interfaces;
using Dekofar.HyperConnect.Application.SupportTickets.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Dekofar.HyperConnect.Application.SupportTickets.Queries
{
    public class GetMyTicketsQueryHandler : IRequestHandler<GetMyTicketsQuery, List<SupportTicketDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;

        public GetMyTicketsQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        public async Task<List<SupportTicketDto>> Handle(GetMyTicketsQuery request, CancellationToken cancellationToken)
        {
            if (_currentUser.UserId == null)
                throw new UnauthorizedAccessException();

            return await _context.SupportTickets
                .Where(t => t.CreatedByUserId == _currentUser.UserId)
                .Select(t => new SupportTicketDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    CreatedByUserId = t.CreatedByUserId,
                    AssignedUserId = t.AssignedUserId,
                    CategoryId = t.CategoryId,
                    OrderId = t.OrderId,
                    Status = t.Status,
                    Priority = t.Priority,
                    DueDate = t.DueDate,
                    FilePath = t.FilePath,
                    CreatedAt = t.CreatedAt,
                    LastUpdatedAt = t.LastUpdatedAt,
                    UnreadReplyCount = t.UnreadReplyCount
                })
                .ToListAsync(cancellationToken);
        }
    }
}
