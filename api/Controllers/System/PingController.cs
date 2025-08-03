using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dekofar.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    // Servisin canlılık durumunu kontrol etmek için basit ping controller
    public class PingController : ControllerBase
    {
        // Herkese açık ping endpoint'i
        [HttpGet("open")]
        public IActionResult OpenPing() => Ok("🌐 Bu endpoint herkese açık.");

        // Sadece geçerli JWT token’ı olanlar erişebilir
        [Authorize]
        [HttpGet("secure")]
        public IActionResult SecurePing() => Ok("🔐 Token doğrulandı, erişim sağlandı!");
    }
}
