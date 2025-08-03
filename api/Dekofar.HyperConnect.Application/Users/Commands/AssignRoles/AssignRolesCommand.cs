using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace Dekofar.HyperConnect.Application.Users.Commands.AssignRoles
{
    public record AssignRolesCommand(Guid UserId, IList<string> Roles) : IRequest<IdentityResult>;
}
