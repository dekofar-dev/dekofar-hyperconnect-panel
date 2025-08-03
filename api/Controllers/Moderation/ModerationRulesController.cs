using System.Threading.Tasks;
using Dekofar.HyperConnect.Application.ModerationRules.Commands;
using Dekofar.HyperConnect.Application.ModerationRules.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dekofar.API.Controllers
{
    [ApiController]
    [Route("api/moderation-rules")]
    [Authorize(Roles = "Admin")]
    public class ModerationRulesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ModerationRulesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var rules = await _mediator.Send(new GetModerationRulesQuery());
            return Ok(rules);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateModerationRuleCommand command)
        {
            var id = await _mediator.Send(command);
            return Ok(id);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateModerationRuleCommand command)
        {
            if (id != command.Id) return BadRequest();
            await _mediator.Send(command);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _mediator.Send(new DeleteModerationRuleCommand(id));
            return Ok();
        }
    }
}
