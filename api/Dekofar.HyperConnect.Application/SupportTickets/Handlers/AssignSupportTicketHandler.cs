using Dekofar.HyperConnect.Application.AuditLogs.Commands;
using Dekofar.HyperConnect.Application.Common.Interfaces;
using Dekofar.HyperConnect.Application.SupportTickets.Commands;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Dekofar.HyperConnect.Application.SupportTickets.Handlers
{
    public class AssignSupportTicketHandler : IRequestHandler<AssignSupportTicketCommand, Unit>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMediator _mediator;

        public AssignSupportTicketHandler(IApplicationDbContext context, IMediator mediator)
        {
            _context = context;
            _mediator = mediator;
        }

        public async Task<Unit> Handle(AssignSupportTicketCommand request, CancellationToken cancellationToken)
        {
            var ticket = await _context.SupportTickets.FindAsync(new object?[] { request.TicketId }, cancellationToken);
            if (ticket == null)
                throw new Exception("Ticket not found");

            ticket.AssignedUserId = request.AssignedUserId;
            ticket.LastUpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);

            await _mediator.Send(new CreateAuditLogCommand
            {
                Action = "AssignTicket",
                TargetType = "SupportTicket",
                TargetId = ticket.Id,
                Description = $"Assigned to user {request.AssignedUserId}"
            }, cancellationToken);

            return Unit.Value;
        }
    }
}
