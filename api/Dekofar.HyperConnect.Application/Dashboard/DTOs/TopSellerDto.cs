using System;

namespace Dekofar.HyperConnect.Application.Dashboard.DTOs
{
    public class TopSellerDto
    {
        public Guid UserId { get; set; }
        public string? FullName { get; set; }
        public decimal TotalSales { get; set; }
    }
}
