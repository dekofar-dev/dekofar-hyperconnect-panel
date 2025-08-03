using Dekofar.HyperConnect.Integrations.NetGsm.Interfaces;
using Dekofar.HyperConnect.Integrations.NetGsm.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Dekofar.HyperConnect.Integrations.NetGsm.Services
{
    public class NetGsmCallService : INetGsmCallService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<NetGsmCallService> _logger;
        private readonly HttpClient _httpClient;

        public NetGsmCallService(IConfiguration configuration, ILogger<NetGsmCallService> logger)
        {
            _configuration = configuration;
            _logger = logger;
            _httpClient = new HttpClient();
        }

        public async Task<string> GetCallLogsAsync(CallLogRequest request)
        {
            var username = _configuration["NetGsm:Username"];
            var password = _configuration["NetGsm:Password"];
            var appkey = _configuration["NetGsm:AppKey"];

            var xml = $@"<?xml version='1.0' encoding='UTF-8'?>
<mainbody>
   <header>
       <usercode>{username}</usercode>
       <password>{password}</password>
       <date>{request.Date}</date>
       <direction>{request.Direction}</direction>
       <appkey>{appkey}</appkey>
   </header>
</mainbody>";

            var content = new StringContent(xml, Encoding.UTF8, "text/xml");

            _logger.LogInformation("📞 NetGSM Call Log Request Sent: {Xml}", xml);

            try
            {
                var response = await _httpClient.PostAsync("https://api.netgsm.com.tr/voice/report", content);
                var result = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("❌ NetGSM Call Log Error: {StatusCode} - {Result}", response.StatusCode, result);
                    throw new Exception("NetGSM servisi başarısız yanıt döndü.");
                }

                _logger.LogInformation("📞 NetGSM Call Log Response: {Result}", result);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ NetGSM Call Log Exception");
                throw;
            }
        }
    }
}
