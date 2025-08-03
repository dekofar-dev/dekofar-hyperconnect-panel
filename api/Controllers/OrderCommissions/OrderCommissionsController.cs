using Dekofar.HyperConnect.Application.OrderCommissions.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Dekofar.API.Controllers
{
    [ApiController]
    [Route("api/order-commissions")]
    [Authorize]
    // Sipariş komisyonlarını yöneten controller
    public class OrderCommissionsController : ControllerBase
    {
        // MediatR aracısı
        private readonly IMediator _mediator;

        // MediatR bağımlılığını alan kurucu
        public OrderCommissionsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // Mevcut kullanıcının komisyonlarını listeler
        [HttpGet("user")]
        public async Task<IActionResult> GetForCurrentUser()
        {
            var commissions = await _mediator.Send(new GetCommissionsByUserQuery());
            return Ok(commissions);
        }

        // Mevcut kullanıcının toplam komisyon tutarını döner
        [HttpGet("user/total")]
        public async Task<IActionResult> GetTotalForCurrentUser()
        {
            var total = await _mediator.Send(new GetUserTotalCommissionQuery());
            return Ok(total);
        }
    }
}
