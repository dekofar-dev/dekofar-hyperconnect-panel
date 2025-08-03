using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dekofar.HyperConnect.Integrations.NetGsm.Models.Dto
{
    public class SmsInboxResponse
    {
        public string Orjinator { get; set; }
        public string Message { get; set; }
        public string Date { get; set; }
    }

}
