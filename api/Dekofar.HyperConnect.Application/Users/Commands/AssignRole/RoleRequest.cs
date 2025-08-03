using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dekofar.HyperConnect.Application.Users.Commands.AssignRole
{
    public class RoleRequest
    {
        public Guid UserId { get; set; }
        public string RoleName { get; set; } = null!;
    }
}
