using Dekofar.HyperConnect.Application.SupportTickets.Commands;
using Dekofar.HyperConnect.Application.SupportTickets.Queries;
using Dekofar.API.Authorization;
using Dekofar.API.Hubs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;
using Dekofar.HyperConnect.Domain.Entities;

namespace Dekofar.API.Controllers
{
    [ApiController]
    [Route("api/support-tickets")]
    [Authorize]
    // Destek talebi işlemlerini yöneten controller
    public class SupportTicketsController : ControllerBase
    {
        // MediatR aracısı
        private readonly IMediator _mediator;
        private readonly IHubContext<SupportHub> _hubContext;

        // MediatR bağımlılığını alan kurucu
        public SupportTicketsController(IMediator mediator, IHubContext<SupportHub> hubContext)
        {
            _mediator = mediator;
            _hubContext = hubContext;
        }

        // Yeni destek talebi oluşturur
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Create([FromForm] CreateSupportTicketCommand command)
        {
            var id = await _mediator.Send(command);
            await _hubContext.Clients.Group("SupportAgents").SendAsync("OnSupportTicketCreated", new { ticketId = id });
            return Ok(id);
        }

        /// <summary>
        /// Destek taleplerini listeler.
        /// Yönetici kullanıcılar tüm talepleri görebilirken diğer kullanıcılar
        /// sadece kendi oluşturduğu veya kendisine atanan talepleri görebilir.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] Guid? category = null,
            [FromQuery] SupportTicketStatus? status = null,
            [FromQuery] string? search = null)
        {
            var userId = User.GetUserId();
            if (userId == null) return Unauthorized();

            // İstemciden gelen parametreler ile sorgu nesnesi oluşturulur
            var query = new GetAllSupportTicketsQuery
            {
                UserId = userId.Value,
                IsAdmin = User.IsInRole("Admin"),
                PageNumber = pageNumber,
                PageSize = pageSize,
                CategoryId = category,
                Status = status,
                Search = search
            };

            var tickets = await _mediator.Send(query);
            return Ok(tickets);
        }

        // Kullanıcının kendi taleplerini döner
        [HttpGet("my")]
        public async Task<IActionResult> GetMyTickets()
        {
            var tickets = await _mediator.Send(new GetMyTicketsQuery());
            return Ok(tickets);
        }

        // Belirli bir talebi id ile getirir
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var ticket = await _mediator.Send(new GetTicketByIdQuery(id));
            if (ticket == null) return NotFound();
            return Ok(ticket);
        }

        // Destek talebini bir personele atar
        [HttpPost("{id}/assign")]
        [Authorize(Policy = "CanAssignTicket")]
        public async Task<IActionResult> Assign(Guid id, [FromBody] AssignSupportTicketCommand command)
        {
            if (id != command.TicketId) return BadRequest();
            // Policy based authorization ensures only users with the
            // CanAssignTicket permission can reach this point.
            await _mediator.Send(command);
            return Ok();
        }

        // Talebin durumunu günceller
        [HttpPost("{id}/status")]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateSupportTicketStatusCommand command)
        {
            if (id != command.TicketId) return BadRequest();
            // The command encapsulates business rules to move between
            // Open -> InProgress -> Closed states.
            await _mediator.Send(command);
            return Ok();
        }

        // Talebe yanıt ekler
        [HttpPost("{id}/reply")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Reply(Guid id, [FromForm] ReplySupportTicketCommand command)
        {
            if (id != command.TicketId) return BadRequest();
            var reply = await _mediator.Send(command);

            if (User.IsInRole("Admin") || User.IsInRole("Support"))
            {
                await _hubContext.Clients.Group($"user-{reply.TicketOwnerId}").SendAsync("OnSupportTicketReplied", reply);
            }
            else
            {
                await _hubContext.Clients.Group("SupportAgents").SendAsync("OnSupportTicketReplied", reply);
            }

            return Ok(reply);
        }
    }
}
