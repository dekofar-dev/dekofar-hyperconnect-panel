using MediatR;
using System.Collections.Generic;

namespace Dekofar.HyperConnect.Application.Roles.Queries.GetRoles
{
    public record GetRolesQuery : IRequest<List<string>>;
}
