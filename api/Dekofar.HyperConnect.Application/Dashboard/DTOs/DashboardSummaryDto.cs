using System.Collections.Generic;

namespace Dekofar.HyperConnect.Application.Dashboard.DTOs
{
    public class DashboardSummaryDto
    {
        public int TotalUsers { get; set; }
        public int ActiveUsers { get; set; }
        public decimal TotalSales { get; set; }
        public int TotalOrders { get; set; }
        public int TotalSupportTickets { get; set; }
        public int OpenTickets { get; set; }
        public decimal TotalCommissionPaid { get; set; }
        public List<TopSellerDto> TopSellers { get; set; } = new();
    }
}
