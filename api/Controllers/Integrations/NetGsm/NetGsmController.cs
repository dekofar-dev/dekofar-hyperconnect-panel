using Dekofar.HyperConnect.Integrations.NetGsm.Interfaces;
using Dekofar.HyperConnect.Integrations.NetGsm.Models;
using Dekofar.HyperConnect.Integrations.NetGsm.Models.Dto;
using Microsoft.AspNetCore.Mvc;

namespace Dekofar.API.Controllers.Integrations
{
    [ApiController]
    [Route("api/[controller]")]
    // NetGSM servisleri ile haberleşen controller
    public class NetGsmController : ControllerBase
    {
        // SMS gönderimi ve alımı için servis
        private readonly INetGsmSmsService _smsService;
        // Arama kayıtlarına erişim servisi
        private readonly INetGsmCallService _callService;

        // Servis bağımlılıklarını alan kurucu
        public NetGsmController(INetGsmSmsService smsService, INetGsmCallService callService)
        {
            _smsService = smsService;
            _callService = callService;
        }

        // Gelen SMS kutusunu getirir (NetGSM inbox)
        [HttpPost("sms-inbox")]
        public async Task<IActionResult> GetSmsInbox([FromBody] SmsInboxRequest request)
        {
            try
            {
                var result = await _smsService.GetInboxMessagesAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // Belirtilen tarih ve yöne göre çağrı kayıtlarını getirir (NetGSM voice report)
        [HttpPost("call-logs")]
        public async Task<IActionResult> GetCallLogs([FromBody] CallLogRequest request)
        {
            var result = await _callService.GetCallLogsAsync(request);
            return Content(result, "application/xml");
        }
    }
}

