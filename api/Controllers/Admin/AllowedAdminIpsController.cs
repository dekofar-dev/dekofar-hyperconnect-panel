using Dekofar.HyperConnect.Application.AllowedAdminIps.DTOs;
using Dekofar.HyperConnect.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Dekofar.API.Controllers
{
    [ApiController]
    [Route("api/admin/allowed-ips")]
    [Authorize(Roles = "Admin")]
    // Yönetici paneline erişimine izin verilen IP adreslerini yöneten controller
    public class AllowedAdminIpsController : ControllerBase
    {
        // IP servisinin arayüzü
        private readonly IAllowedAdminIpService _service;

        // Servisi alan kurucu
        public AllowedAdminIpsController(IAllowedAdminIpService service)
        {
            _service = service;
        }

        // Tüm izinli IP adreslerini listeler
        [HttpGet]
        public async Task<ActionResult<List<AllowedAdminIpDto>>> GetAll()
        {
            var items = await _service.GetAllAsync();
            return Ok(items);
        }

        // Belirli bir izinli IP kaydını getirir
        [HttpGet("{id}")]
        public async Task<ActionResult<AllowedAdminIpDto>> GetById(int id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        // Yeni bir izinli IP kaydı oluşturur
        [HttpPost]
        public async Task<ActionResult<AllowedAdminIpDto>> Create([FromBody] AllowedAdminIpDto dto)
        {
            var created = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        // Var olan izinli IP kaydını günceller
        [HttpPut("{id}")]
        public async Task<ActionResult<AllowedAdminIpDto>> Update(int id, [FromBody] AllowedAdminIpDto dto)
        {
            var updated = await _service.UpdateAsync(id, dto);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        // Bir izinli IP kaydını siler
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.DeleteAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }
    }
}
