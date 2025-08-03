using System;

namespace Dekofar.HyperConnect.Application.ResponseTemplates.DTOs
{
    public class ResponseTemplateDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = default!;
        public string Body { get; set; } = default!;
        public bool IsGlobal { get; set; }
        public Guid? CreatedByUserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? ModuleScope { get; set; }
    }
}
