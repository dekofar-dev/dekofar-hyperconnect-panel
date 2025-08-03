using System;
using System.Linq;
using System.Threading.Tasks;
using Dekofar.HyperConnect.Domain.Entities;
using Dekofar.HyperConnect.Application.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Dekofar.HyperConnect.API.Authorization
{
    // Authorization handler that verifies the current user's permissions
    public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IApplicationDbContext _context;

        public PermissionAuthorizationHandler(UserManager<ApplicationUser> userManager, IApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            // Extract the user identifier from the JWT claims
            var userId = context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                // No authenticated user
                return;
            }

            // Load the user and their roles from Identity
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return;
            }
            var roles = await _userManager.GetRolesAsync(user);

            // Fetch the permission along with roles that have it
            var permission = await _context.Permissions
                .Include(p => p.RolePermissions)
                .FirstOrDefaultAsync(p => p.Name == requirement.Permission);

            if (permission == null)
                return;

            // Check if any of the user's roles owns the permission
            var hasPermission = permission.RolePermissions != null &&
                                permission.RolePermissions.Any(rp => roles.Contains(rp.RoleName));

            if (hasPermission)
            {
                // Mark the requirement as satisfied
                context.Succeed(requirement);
            }
        }
    }
}
