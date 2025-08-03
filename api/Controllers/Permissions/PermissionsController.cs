using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dekofar.HyperConnect.Application.Common.Interfaces;
using Dekofar.HyperConnect.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dekofar.API.Controllers
{
    [ApiController]
    [Route("api")]
    // Yetki ve izin yönetimini sağlayan controller
    public class PermissionsController : ControllerBase
    {
        // Veritabanı erişimi için uygulama bağlamı
        private readonly IApplicationDbContext _context;
        // Rol yönetimi için Identity RoleManager servisi
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;

        // Bağımlılıkları alan kurucu metot
        public PermissionsController(IApplicationDbContext context, RoleManager<IdentityRole<Guid>> roleManager)
        {
            _context = context;
            _roleManager = roleManager;
        }

        // Sistemde tanımlı tüm izinleri döner
        [HttpGet("permissions")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var permissions = await _context.Permissions.AsNoTracking().ToListAsync();
            return Ok(permissions);
        }

        // Bir rolün izinlerini verilen liste ile değiştirir
        [HttpPost("roles/{role}/permissions")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AssignToRole(string role, [FromBody] List<Guid> permissionIds)
        {
            var roleEntity = await _roleManager.FindByNameAsync(role);
            if (roleEntity == null)
                return NotFound();

            // Mevcut atamaları kaldır ve yenilerini ekle
            var existing = _context.RolePermissions.Where(rp => rp.RoleName == role);
            _context.RolePermissions.RemoveRange(existing);

            foreach (var id in permissionIds)
            {
                _context.RolePermissions.Add(new RolePermission
                {
                    Id = Guid.NewGuid(),
                    RoleName = role,
                    PermissionId = id
                });
            }

            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
