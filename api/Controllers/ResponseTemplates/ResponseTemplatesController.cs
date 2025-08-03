using System.Threading.Tasks;
using Dekofar.HyperConnect.Application.ResponseTemplates.Commands;
using Dekofar.HyperConnect.Application.ResponseTemplates.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dekofar.API.Controllers
{
    [ApiController]
    [Route("api/response-templates")]
    [Authorize]
    // Yanıt şablonları ile ilgili CRUD işlemlerini yöneten controller
    public class ResponseTemplatesController : ControllerBase
    {
        // MediatR aracısı
        private readonly IMediator _mediator;

        // MediatR bağımlılığını alan kurucu
        public ResponseTemplatesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // Tüm yanıt şablonlarını getirir
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] string? module)
        {
            var templates = await _mediator.Send(new GetResponseTemplatesQuery(module));
            return Ok(templates);
        }

        // Id'ye göre yanıt şablonu getirir
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var template = await _mediator.Send(new GetResponseTemplateByIdQuery(id));
            if (template == null)
                return NotFound(); // Şablon yoksa 404 döner

            return Ok(template);
        }

        // Yeni yanıt şablonu oluşturur
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateResponseTemplateCommand command)
        {
            if (command.IsGlobal && !User.IsInRole("Admin"))
                return Forbid(); // Global şablon için sadece admin yetkisi

            var id = await _mediator.Send(command);
            return Ok(id);
        }

        // Mevcut bir şablonu günceller
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateResponseTemplateCommand command)
        {
            if (id != command.Id) return BadRequest(); // Id eşleşmezse 400 döner

            var existing = await _mediator.Send(new GetResponseTemplateByIdQuery(id));
            if (existing == null) return NotFound(); // Şablon yoksa 404 döner
            if ((existing.IsGlobal || command.IsGlobal) && !User.IsInRole("Admin"))
                return Forbid(); // Global şablon sadece admin tarafından değiştirilebilir

            await _mediator.Send(command);
            return Ok();
        }

        // Bir şablonu siler
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _mediator.Send(new GetResponseTemplateByIdQuery(id));
            if (existing == null) return NotFound(); // Şablon yoksa 404 döner
            if (existing.IsGlobal && !User.IsInRole("Admin"))
                return Forbid(); // Global şablonu sadece admin silebilir

            await _mediator.Send(new DeleteResponseTemplateCommand(id));
            return Ok();
        }
    }
}
