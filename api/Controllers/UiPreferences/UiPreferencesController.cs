using Dekofar.HyperConnect.Application.UIPreferences.Commands;
using Dekofar.HyperConnect.Application.UIPreferences.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Dekofar.API.Controllers
{
    [ApiController]
    [Route("api/ui-preferences")]
    [Authorize]
    public class UiPreferencesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UiPreferencesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{moduleKey}")]
        public async Task<IActionResult> Get(string moduleKey)
        {
            var result = await _mediator.Send(new GetUserUiPreferenceQuery(moduleKey));
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Upsert([FromBody] UpsertUserUiPreferenceCommand command)
        {
            await _mediator.Send(command);
            return NoContent();
        }
    }
}
