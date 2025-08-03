using System.Collections.Generic;

namespace Dekofar.HyperConnect.Application.Common.Dtos
{
    public class BulkActionDto
    {
        public List<int> Ids { get; set; } = new();
        public string Action { get; set; } = string.Empty;
        public string? AdminId { get; set; }
    }
}
