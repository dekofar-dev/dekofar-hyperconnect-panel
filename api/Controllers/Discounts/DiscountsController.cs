using Dekofar.HyperConnect.Application.Discounts.Commands;
using Dekofar.HyperConnect.Application.Discounts.DTOs;
using Dekofar.HyperConnect.Application.Discounts.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dekofar.API.Controllers
{
    [ApiController]
    [Route("api/discounts")]
    // İndirim işlemlerini yöneten controller
    public class DiscountsController : ControllerBase
    {
        // Komut ve sorguları çalıştırmak için MediatR aracı
        private readonly IMediator _mediator;

        // MediatR bağımlılığını alan kurucu
        public DiscountsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // Tüm indirimleri listeler
        [HttpGet]
        public async Task<ActionResult<List<DiscountDto>>> GetAll()
        {
            var discounts = await _mediator.Send(new GetAllDiscountsQuery());
            return Ok(discounts);
        }

        // Sadece aktif indirimleri listeler
        [HttpGet("active")]
        public async Task<ActionResult<List<DiscountDto>>> GetActive()
        {
            var discounts = await _mediator.Send(new GetAllDiscountsQuery(true));
            return Ok(discounts);
        }

        // Yeni indirim oluşturur
        [HttpPost]
        [Authorize(Policy = "CanManageDiscounts")]
        public async Task<IActionResult> Create([FromBody] CreateDiscountCommand command)
        {
            var id = await _mediator.Send(command);
            return Ok(id);
        }

        // Mevcut bir indirimi günceller
        [HttpPut("{id}")]
        [Authorize(Policy = "CanManageDiscounts")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateDiscountCommand command)
        {
            if (id != command.Id) return BadRequest();
            await _mediator.Send(command);
            return Ok();
        }

        // İndirimi siler
        [HttpDelete("{id}")]
        [Authorize(Policy = "CanManageDiscounts")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _mediator.Send(new DeleteDiscountCommand(id));
            return Ok();
        }
    }
}
