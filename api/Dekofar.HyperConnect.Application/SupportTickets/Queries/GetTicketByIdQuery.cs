using Dekofar.HyperConnect.Application.SupportTickets.DTOs;
using MediatR;
using System;

namespace Dekofar.HyperConnect.Application.SupportTickets.Queries
{
    public class GetTicketByIdQuery : IRequest<SupportTicketDto?>
    {
        public Guid Id { get; }
        public GetTicketByIdQuery(Guid id) => Id = id;
    }
}
