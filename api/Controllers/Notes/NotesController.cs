using Dekofar.HyperConnect.Application.Notes.Commands;
using Dekofar.HyperConnect.Application.Notes.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Dekofar.API.Controllers
{
    [ApiController]
    [Route("api/notes")]
    [Authorize]
    // Not işlemlerini yöneten controller
    public class NotesController : ControllerBase
    {
        // MediatR aracısı
        private readonly IMediator _mediator;

        // MediatR bağımlılığını alan kurucu
        public NotesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // Belirli bir hedefe ait notları getirir
        [HttpGet("{targetType}/{targetId}")]
        public async Task<IActionResult> GetByTarget(string targetType, Guid targetId)
        {
            var notes = await _mediator.Send(new GetNotesByTargetQuery(targetType, targetId));
            return Ok(notes);
        }

        // Yeni not ekler
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] AddNoteCommand command)
        {
            var id = await _mediator.Send(command);
            return Ok(id);
        }
    }
}
