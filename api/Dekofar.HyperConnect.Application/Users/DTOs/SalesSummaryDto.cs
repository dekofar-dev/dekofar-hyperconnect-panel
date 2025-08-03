namespace Dekofar.HyperConnect.Application.Users.DTOs
{
    public class SalesSummaryDto
    {
        public decimal TotalSalesAmount { get; set; }
        public decimal TotalCommissionAmount { get; set; }
        public int OrdersCount { get; set; }
        public int CommissionCount { get; set; }
    }
}
