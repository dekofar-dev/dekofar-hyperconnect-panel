using MediatR;
using Microsoft.AspNetCore.Identity;
using System;

namespace Dekofar.HyperConnect.Application.Roles.Commands.CreateRole
{
    public record CreateRoleCommand(string RoleName) : IRequest<IdentityResult>;
}
