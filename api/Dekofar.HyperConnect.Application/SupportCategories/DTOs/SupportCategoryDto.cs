using System;
using System.Collections.Generic;

namespace Dekofar.HyperConnect.Application.SupportCategories.DTOs
{
    public class SupportCategoryDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<string> Roles { get; set; } = new();
    }
}
