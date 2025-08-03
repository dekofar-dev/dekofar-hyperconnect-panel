using System;

namespace Dekofar.HyperConnect.Domain.Entities
{
    public class DeploymentLog
    {
        public int Id { get; set; }
        public string Version { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string DeployedBy { get; set; } = string.Empty;
        public DateTime DeployedAt { get; set; } = DateTime.UtcNow;
    }
}
