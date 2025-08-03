using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dekofar.HyperConnect.Integrations.NetGsm.Models.Dto
{
    public class SmsInboxRequest
    {
        public string StartDate { get; set; }  // "250620250000"
        public string StopDate { get; set; }   // "250720252359"
    }

}
