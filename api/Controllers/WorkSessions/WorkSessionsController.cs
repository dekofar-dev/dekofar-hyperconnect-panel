using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Dekofar.HyperConnect.Application.Common.Interfaces;
using Dekofar.HyperConnect.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dekofar.API.Controllers
{
    /// <summary>
    /// Kullanıcı çalışma oturumlarını yöneten controller.
    /// </summary>
    [ApiController]
    [Route("api/work-sessions")]
    public class WorkSessionsController : ControllerBase
    {
        private readonly IWorkSessionService _workSessionService;

        public WorkSessionsController(IWorkSessionService workSessionService)
        {
            _workSessionService = workSessionService;
        }

        // Bu endpoint kullanıcı oturumunu başlatmak için kullanılır.
        // Erişim: Giriş yapmış tüm kullanıcılar.
        // Girdi: Yok, çıktı: oluşturulan WorkSession
        [HttpPost("start")]
        [Authorize]
        public async Task<ActionResult<WorkSession>> Start()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();
            var session = await _workSessionService.StartSessionAsync(Guid.Parse(userId), HttpContext.Connection.RemoteIpAddress?.ToString());
            return Ok(session);
        }

        // Bu endpoint aktif oturumu sonlandırır.
        // Erişim: Giriş yapmış tüm kullanıcılar.
        // Girdi: Yok, çıktı: sonlanan WorkSession
        [HttpPost("end")]
        [Authorize]
        public async Task<ActionResult<WorkSession?>> End()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();
            var session = await _workSessionService.EndSessionAsync(Guid.Parse(userId));
            return Ok(session);
        }

        // Bu endpoint mevcut kullanıcının oturum geçmişini listeler.
        // Erişim: Giriş yapmış tüm kullanıcılar.
        // Girdi: Yok, çıktı: WorkSession listesi
        [HttpGet("my")]
        [Authorize]
        public async Task<ActionResult> MySessions()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();
            var sessions = await _workSessionService.GetUserSessionsAsync(Guid.Parse(userId));
            return Ok(sessions);
        }

        // Bu endpoint tüm oturumları rapor olarak döner.
        // Erişim: Admin
        // Girdi: Yok, çıktı: WorkSession listesi
        [HttpGet("report")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Report()
        {
            var sessions = await _workSessionService.GetReportAsync();
            return Ok(sessions);
        }
    }
}
