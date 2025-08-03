using Dekofar.HyperConnect.Domain.Entities;
using MediatR;
using System;

namespace Dekofar.HyperConnect.Application.SupportTickets.Commands
{
    public class UpdateSupportTicketStatusCommand : IRequest<Unit>
    {
        public Guid TicketId { get; set; }
        public SupportTicketStatus Status { get; set; }
    }
}
