using System;

namespace Dekofar.HyperConnect.Domain.Entities
{
    // Join entity mapping roles to permissions
    public class RolePermission
    {
        // Primary key of the join record
        public Guid Id { get; set; }

        // The role that owns the permission (e.g. "Admin")
        public string RoleName { get; set; } = default!;

        // Foreign key to the permission
        public Guid PermissionId { get; set; }

        // Navigation property for EF Core
        public Permission? Permission { get; set; }
    }
}
