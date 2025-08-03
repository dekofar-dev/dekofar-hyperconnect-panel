using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dekofar.HyperConnect.Domain.DTOs
{
    public class ChangeUserRoleRequest
    {
        public string UserId { get; set; }
        public string NewRole { get; set; }
    }
}
