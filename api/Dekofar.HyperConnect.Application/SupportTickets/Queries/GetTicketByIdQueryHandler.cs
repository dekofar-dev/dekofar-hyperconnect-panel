using Dekofar.HyperConnect.Application.Common.Interfaces;
using Dekofar.HyperConnect.Application.SupportTickets.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace Dekofar.HyperConnect.Application.SupportTickets.Queries
{
    public class GetTicketByIdQueryHandler : IRequestHandler<GetTicketByIdQuery, SupportTicketDto?>
    {
        private readonly IApplicationDbContext _context;

        public GetTicketByIdQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<SupportTicketDto?> Handle(GetTicketByIdQuery request, CancellationToken cancellationToken)
        {
            var ticket = await _context.SupportTickets
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

            if (ticket == null) return null;

            return new SupportTicketDto
            {
                Id = ticket.Id,
                Title = ticket.Title,
                Description = ticket.Description,
                CreatedByUserId = ticket.CreatedByUserId,
                AssignedUserId = ticket.AssignedUserId,
                CategoryId = ticket.CategoryId,
                OrderId = ticket.OrderId,
                Status = ticket.Status,
                Priority = ticket.Priority,
                DueDate = ticket.DueDate,
                FilePath = ticket.FilePath,
                CreatedAt = ticket.CreatedAt,
                LastUpdatedAt = ticket.LastUpdatedAt,
                UnreadReplyCount = ticket.UnreadReplyCount
            };
        }
    }
}
