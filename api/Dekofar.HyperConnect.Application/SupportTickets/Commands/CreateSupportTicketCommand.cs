using Dekofar.HyperConnect.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

namespace Dekofar.HyperConnect.Application.SupportTickets.Commands
{
    public class CreateSupportTicketCommand : IRequest<Guid>
    {
        [Required]
        public string Title { get; set; } = default!;
        [Required]
        public string Description { get; set; } = default!;
        public Guid? CategoryId { get; set; }
        public Guid? OrderId { get; set; }
        public SupportTicketPriority Priority { get; set; } = SupportTicketPriority.Medium;
        public DateTime? DueDate { get; set; }
        public IFormFile? File { get; set; }
    }
}
