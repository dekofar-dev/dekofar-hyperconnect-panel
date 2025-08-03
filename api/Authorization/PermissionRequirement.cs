using Microsoft.AspNetCore.Authorization;

namespace Dekofar.HyperConnect.API.Authorization
{
    // Represents a requirement for a specific permission value
    public class PermissionRequirement : IAuthorizationRequirement
    {
        // Name of the permission that must be satisfied
        public string Permission { get; }

        public PermissionRequirement(string permission)
        {
            // Capture the permission string for later evaluation
            Permission = permission;
        }
    }
}
