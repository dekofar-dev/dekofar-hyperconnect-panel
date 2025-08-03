using MediatR;
using System;

namespace Dekofar.HyperConnect.Application.AuditLogs.Commands
{
    public class CreateAuditLogCommand : IRequest<Unit>
    {
        public string Action { get; set; } = default!;
        public string TargetType { get; set; } = default!;
        public Guid TargetId { get; set; }
        public string Description { get; set; } = default!;
    }
}
