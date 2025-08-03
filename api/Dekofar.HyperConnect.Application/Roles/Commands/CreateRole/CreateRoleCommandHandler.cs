using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Dekofar.HyperConnect.Application.Roles.Commands.CreateRole
{
    public class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand, IdentityResult>
    {
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;

        public CreateRoleCommandHandler(RoleManager<IdentityRole<Guid>> roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task<IdentityResult> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
        {
            if (await _roleManager.RoleExistsAsync(request.RoleName))
            {
                return IdentityResult.Failed(new IdentityError { Description = $"Role '{request.RoleName}' already exists." });
            }

            var role = new IdentityRole<Guid>(request.RoleName);
            return await _roleManager.CreateAsync(role);
        }
    }
}
