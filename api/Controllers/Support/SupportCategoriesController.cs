using Dekofar.HyperConnect.Application.SupportCategories.Commands;
using Dekofar.HyperConnect.Application.SupportCategories.DTOs;
using Dekofar.HyperConnect.Application.SupportCategories.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dekofar.API.Controllers
{
    [ApiController]
    [Route("api/support-categories")]
    [Authorize]
    // Destek kategori işlemlerini yöneten controller
    public class SupportCategoriesController : ControllerBase
    {
        // MediatR aracısı
        private readonly IMediator _mediator;

        // MediatR'ı alan kurucu
        public SupportCategoriesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // Tüm destek kategorilerini listeler
        [HttpGet]
        public async Task<ActionResult<List<SupportCategoryDto>>> GetAll()
        {
            var categories = await _mediator.Send(new GetAllSupportCategoriesQuery());
            return Ok(categories);
        }

        // Yeni destek kategorisi oluşturur
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateSupportCategoryCommand command)
        {
            var id = await _mediator.Send(command);
            return Ok(id);
        }

        // Destek kategorisini günceller
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSupportCategoryCommand command)
        {
            if (id != command.Id) return BadRequest();
            await _mediator.Send(command);
            return Ok();
        }

        // Destek kategorisini siler
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _mediator.Send(new DeleteSupportCategoryCommand(id));
            return Ok();
        }

        // Rol atama isteğini temsil eden DTO
        public class AssignRolesDto
        {
            // Kategoriye atanacak roller listesi
            public List<string> Roles { get; set; } = new();
        }

        // Belirtilen kategoriye roller atar
        [HttpPost("{id}/roles")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AssignRoles(Guid id, [FromBody] AssignRolesDto dto)
        {
            var command = new AssignRolesToSupportCategoryCommand
            {
                SupportCategoryId = id,
                Roles = dto.Roles
            };
            await _mediator.Send(command);
            return Ok();
        }
    }
}
