using Dekofar.HyperConnect.Application.Common.Interfaces;
using Dekofar.HyperConnect.Application.SupportTickets.Commands;
using Dekofar.HyperConnect.Domain.Entities;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Dekofar.HyperConnect.Application.SupportTickets.Handlers
{
    public class UpdateSupportTicketStatusHandler : IRequestHandler<UpdateSupportTicketStatusCommand, Unit>
    {
        private readonly IApplicationDbContext _context;

        public UpdateSupportTicketStatusHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(UpdateSupportTicketStatusCommand request, CancellationToken cancellationToken)
        {
            var ticket = await _context.SupportTickets.FindAsync(new object?[] { request.TicketId }, cancellationToken);
            if (ticket == null)
                throw new Exception("Ticket not found");

            ticket.Status = request.Status;
            ticket.LastUpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}
