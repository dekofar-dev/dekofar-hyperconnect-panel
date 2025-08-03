using Dekofar.HyperConnect.Application.ManualOrders.Commands;
using Dekofar.HyperConnect.Application.Common.Interfaces;
using Dekofar.HyperConnect.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dekofar.API.Controllers
{
    [ApiController]
    [Route("api/manual-orders")]
    [Authorize(Roles = "Admin")]
    // Manuel siparişlerle ilgili CRUD işlemlerini yöneten controller
    public class ManualOrdersController : ControllerBase
    {
        // MediatR aracısı
        private readonly IMediator _mediator;
        // Veritabanı bağlamı
        private readonly IApplicationDbContext _context;

        // MediatR ve veritabanı bağlamını alan kurucu metot
        public ManualOrdersController(IMediator mediator, IApplicationDbContext context)
        {
            _mediator = mediator;
            _context = context;
        }

        // Tüm manuel siparişleri getirir
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            // Veritabanından tüm manuel siparişleri çeker
            var orders = await _context.ManualOrders.AsNoTracking().ToListAsync();
            return Ok(orders);
        }

        // Belirli bir manuel siparişi Id ile getirir
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            // Id'ye göre manuel siparişi arar
            var order = await _context.ManualOrders.FindAsync(id);
            if (order == null)
                return NotFound(); // Sipariş bulunamazsa 404 döner

            return Ok(order);
        }

        // Yeni manuel sipariş oluşturur
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateManualOrderCommand command)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState); // Model geçersizse 400 döner

            var id = await _mediator.Send(command); // MediatR ile komut gönderilir
            return Ok(id);
        }

        // Mevcut bir manuel siparişi günceller
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] ManualOrder order)
        {
            if (id != order.Id)
                return BadRequest(); // Id eşleşmezse 400 döner

            var existing = await _context.ManualOrders.FindAsync(id);
            if (existing == null)
                return NotFound(); // Güncellenecek sipariş yoksa 404 döner

            // Mevcut siparişin alanlarını günceller
            existing.CustomerName = order.CustomerName;
            existing.CustomerSurname = order.CustomerSurname;
            existing.Phone = order.Phone;
            existing.Email = order.Email;
            existing.Address = order.Address;
            existing.City = order.City;
            existing.District = order.District;
            existing.PaymentType = order.PaymentType;
            existing.OrderNote = order.OrderNote;
            existing.Status = order.Status;
            existing.TotalAmount = order.TotalAmount;
            existing.DiscountName = order.DiscountName;
            existing.DiscountType = order.DiscountType;
            existing.DiscountValue = order.DiscountValue;
            existing.BonusAmount = order.BonusAmount;

            await _context.SaveChangesAsync(); // Değişiklikleri kaydeder
            return Ok();
        }

        // Belirli bir manuel siparişi siler
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var existing = await _context.ManualOrders.FindAsync(id);
            if (existing == null)
                return NotFound(); // Silinecek sipariş bulunamazsa 404 döner

            _context.ManualOrders.Remove(existing); // Siparişi siler
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}

