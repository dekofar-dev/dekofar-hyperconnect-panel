using Dekofar.HyperConnect.Integrations.NetGsm.Interfaces;
using Dekofar.HyperConnect.Integrations.NetGsm.Models.Dto;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Linq;
using System;

namespace Dekofar.HyperConnect.Integrations.NetGsm.Services
{
    public class NetGsmSmsService : INetGsmSmsService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<NetGsmSmsService> _logger;
        private readonly HttpClient _httpClient;

        public NetGsmSmsService(IConfiguration configuration, ILogger<NetGsmSmsService> logger)
        {
            _configuration = configuration;
            _logger = logger;
            _httpClient = new HttpClient();
        }

        public async Task<List<SmsInboxResponse>> GetInboxMessagesAsync(SmsInboxRequest request)
        {
            var usercode = _configuration["NetGsm:Username"];
            var password = _configuration["NetGsm:Password"];
            var baseUrl = _configuration["NetGsm:ReceiveSmsBaseUrl"];

            var xmlBody = new StringBuilder();
            xmlBody.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            xmlBody.AppendLine("<mainbody>");
            xmlBody.AppendLine("  <header>");
            xmlBody.AppendLine($"    <Username>{usercode}</Username>");
            xmlBody.AppendLine($"    <password>{password}</password>");
            xmlBody.AppendLine($"    <startdate>{request.StartDate}</startdate>");
            xmlBody.AppendLine($"    <stopdate>{request.StopDate}</stopdate>");
            xmlBody.AppendLine("    <type>0</type>");
            xmlBody.AppendLine("    <appkey>DekofarHyperConnect</appkey>");
            xmlBody.AppendLine("  </header>");
            xmlBody.AppendLine("</mainbody>");

            var content = new StringContent(xmlBody.ToString(), Encoding.UTF8, "application/xml");

            try
            {
                _logger.LogInformation("📤 NetGSM XML SMS isteği gönderiliyor: {Url}", baseUrl);

                var response = await _httpClient.PostAsync(baseUrl, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                _logger.LogInformation("📥 XML Yanıt: {Xml}", responseContent);

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"NetGSM isteği başarısız: {response.StatusCode}");
                }

                var xml = XDocument.Parse(responseContent);
                var inboxElements = xml.Descendants("inbox");

                var smsList = inboxElements.Select(x => new SmsInboxResponse
                {
                    Orjinator = x.Element("gsmno")?.Value,
                    Message = x.Element("msg")?.Value,
                    Date = x.Element("date")?.Value
                }).ToList();

                return smsList;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ NetGSM SMS çekme hatası");
                throw;
            }
        }
    }
}
