using Dekofar.HyperConnect.Integrations.NetGsm.Models.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dekofar.HyperConnect.Integrations.NetGsm.Interfaces
{
    public interface INetGsmSmsService
    {
        Task<List<SmsInboxResponse>> GetInboxMessagesAsync(SmsInboxRequest request);
    }
}
