using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dekofar.HyperConnect.Integrations.NetGsm.Models
{
    public class CallLogRequest
    {
        public string Date { get; set; } // örn: "20250726"
        public string Direction { get; set; } = "all"; // "in", "out", "all"
    }

}
