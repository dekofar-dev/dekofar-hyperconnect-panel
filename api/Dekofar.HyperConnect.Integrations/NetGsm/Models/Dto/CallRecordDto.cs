using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dekofar.HyperConnect.Integrations.NetGsm.Models.Dto
{
    public class CallRecordDto
    {
        public string Caller { get; set; }           // Arayan
        public string Called { get; set; } = "8503043225"; // Sabit
        public string Direction { get; set; }        // "Gelen" veya "Giden"
        public string Date { get; set; }             // Tarih
        public string Duration { get; set; }         // Süre
    }
}
