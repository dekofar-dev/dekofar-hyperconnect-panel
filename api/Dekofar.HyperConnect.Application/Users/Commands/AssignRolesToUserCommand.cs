using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dekofar.HyperConnect.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Dekofar.HyperConnect.Application.Users.Commands
{
    public class AssignRolesToUserCommand : IRequest<IdentityResult>
    {
        public Guid UserId { get; set; }
        public List<string> Roles { get; set; } = new();
    }

    public class AssignRolesToUserCommandHandler : IRequestHandler<AssignRolesToUserCommand, IdentityResult>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;

        public AssignRolesToUserCommandHandler(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole<Guid>> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IdentityResult> Handle(AssignRolesToUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "User not found." });
            }

            var currentRoles = await _userManager.GetRolesAsync(user);
            var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
            if (!removeResult.Succeeded)
            {
                return removeResult;
            }

            foreach (var role in request.Roles)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    var roleResult = await _roleManager.CreateAsync(new IdentityRole<Guid>(role));
                    if (!roleResult.Succeeded)
                    {
                        return roleResult;
                    }
                }
            }

            return await _userManager.AddToRolesAsync(user, request.Roles);
        }
    }
}
