using System;
using System.Threading;
using System.Threading.Tasks;
using Dekofar.HyperConnect.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Dekofar.HyperConnect.Application.Users.Commands
{
    /// <summary>
    /// Kullanıcının mevcut şifresini değiştirir.
    /// </summary>
    public class ChangePasswordCommand : IRequest<IdentityResult>
    {
        public Guid UserId { get; set; }
        public string CurrentPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }

    public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, IdentityResult>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ChangePasswordCommandHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IdentityResult> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user == null)
                return IdentityResult.Failed(new IdentityError { Description = "Kullanıcı bulunamadı." });
            return await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
        }
    }
}
