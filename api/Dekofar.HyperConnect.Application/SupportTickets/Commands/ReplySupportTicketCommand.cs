using Dekofar.HyperConnect.Application.SupportTickets.DTOs;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;

namespace Dekofar.HyperConnect.Application.SupportTickets.Commands
{
    public class ReplySupportTicketCommand : IRequest<SupportTicketReplyDto>
    {
        public Guid TicketId { get; set; }
        public string Message { get; set; } = default!;
        public IFormFile? File { get; set; }
    }
}
