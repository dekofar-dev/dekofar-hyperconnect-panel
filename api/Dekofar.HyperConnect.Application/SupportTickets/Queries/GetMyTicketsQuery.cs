using Dekofar.HyperConnect.Application.SupportTickets.DTOs;
using MediatR;
using System.Collections.Generic;

namespace Dekofar.HyperConnect.Application.SupportTickets.Queries
{
    public class GetMyTicketsQuery : IRequest<List<SupportTicketDto>>
    {
    }
}
