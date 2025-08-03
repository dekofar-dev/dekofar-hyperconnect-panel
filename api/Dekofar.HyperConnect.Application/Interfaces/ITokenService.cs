using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dekofar.HyperConnect.Application.Interfaces
{
    public interface ITokenService
    {
        string GenerateToken(string userId, string email, IEnumerable<string> roles);
    }
}
