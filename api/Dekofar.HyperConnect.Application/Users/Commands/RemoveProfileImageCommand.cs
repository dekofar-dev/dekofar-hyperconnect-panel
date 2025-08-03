using System;
using System.Threading;
using System.Threading.Tasks;
using Dekofar.HyperConnect.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Dekofar.HyperConnect.Application.Users.Commands
{
    /// <summary>
    /// Kullanıcı profil resmini kaldırmak için kullanılan komut.
    /// </summary>
    public class RemoveProfileImageCommand : IRequest<IdentityResult>
    {
        public Guid UserId { get; set; }
    }

    public class RemoveProfileImageCommandHandler : IRequestHandler<RemoveProfileImageCommand, IdentityResult>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public RemoveProfileImageCommandHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IdentityResult> Handle(RemoveProfileImageCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user == null)
                return IdentityResult.Failed(new IdentityError { Description = "Kullanıcı bulunamadı." });
            user.AvatarUrl = null;
            return await _userManager.UpdateAsync(user);
        }
    }
}
