using System;

namespace Dekofar.HyperConnect.Application.Dashboard.DTOs
{
    /// <summary>
    /// Represents aggregated commission earnings for a given month.
    /// </summary>
    public class MonthlyCommissionDto
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public decimal Total { get; set; }
    }
}
