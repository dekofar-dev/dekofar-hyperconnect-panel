using System;

namespace Dekofar.HyperConnect.Application.Dashboard.DTOs
{
    public class TicketActivityDto
    {
        public DateTime Date { get; set; }
        public int Created { get; set; }
        public int Resolved { get; set; }
    }
}
