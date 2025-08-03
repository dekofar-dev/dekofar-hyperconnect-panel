namespace Dekofar.HyperConnect.Application.Dashboard.DTOs
{
    public class TopProductDto
    {
        public string ProductName { get; set; } = default!;
        public int UnitsSold { get; set; }
        public decimal Revenue { get; set; }
    }
}
