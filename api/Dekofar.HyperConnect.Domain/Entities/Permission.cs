using System;
using System.Collections.Generic;

namespace Dekofar.HyperConnect.Domain.Entities
{
    // Represents a granular action that can be granted to a role
    public class Permission
    {
        // Unique identifier for the permission
        public Guid Id { get; set; }

        // Machine friendly name used inside policies
        public string Name { get; set; } = default!;

        // Optional human friendly description
        public string? Description { get; set; }

        // Roles that currently have this permission
        public ICollection<RolePermission>? RolePermissions { get; set; }
    }
}
