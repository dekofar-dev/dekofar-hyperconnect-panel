using System.Collections.Generic;

namespace Dekofar.HyperConnect.Domain.DTOs
{
    public class AssignRolesRequest
    {
        public IList<string> Roles { get; set; } = new List<string>();
    }
}
