using System;
using System.Threading;
using System.Threading.Tasks;
using Dekofar.HyperConnect.Application.Users.DTOs;
using Dekofar.HyperConnect.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Linq;

namespace Dekofar.HyperConnect.Application.Users.Queries
{
    /// <summary>
    /// Mevcut kullanıcı profilini dönen sorgu.
    /// </summary>
    public class GetUserProfileQuery : IRequest<UserDto?>
    {
        public Guid UserId { get; set; }
    }

    public class GetUserProfileQueryHandler : IRequestHandler<GetUserProfileQuery, UserDto?>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public GetUserProfileQueryHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<UserDto?> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user == null) return null;
            var roles = await _userManager.GetRolesAsync(user);
            return new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                Roles = roles.ToList()
            };
        }
    }
}
