using Dekofar.HyperConnect.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Dekofar.HyperConnect.Infrastructure.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        public Guid? UserId { get; }
        public string? UserName { get; }

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            var httpContext = httpContextAccessor.HttpContext;

            var userIdClaim = httpContext?.User?.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var id))
            {
                UserId = id;
            }

            UserName = httpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;
        }
    }
}
