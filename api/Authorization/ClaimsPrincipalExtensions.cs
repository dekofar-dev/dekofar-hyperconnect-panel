using System;
using System.Security.Claims;

namespace Dekofar.API.Authorization
{
    public static class ClaimsPrincipalExtensions
    {
        public static Guid? GetUserId(this ClaimsPrincipal user)
        {
            var id = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (Guid.TryParse(id, out var guid))
            {
                return guid;
            }
            return null;
        }
    }
}
