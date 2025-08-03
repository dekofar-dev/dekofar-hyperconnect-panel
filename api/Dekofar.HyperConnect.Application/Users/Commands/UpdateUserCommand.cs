using System;
using System.Threading;
using System.Threading.Tasks;
using Dekofar.HyperConnect.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Dekofar.HyperConnect.Application.Users.Commands
{
    /// <summary>
    /// Kullanıcı profil bilgilerini güncellemek için kullanılan komut.
    /// </summary>
    public class UpdateUserCommand : IRequest<IdentityResult>
    {
        // Güncellenecek kullanıcının kimliği
        public Guid Id { get; set; }
        // Yeni tam ad
        public string? FullName { get; set; }
        // Yeni e-posta adresi
        public string? Email { get; set; }
    }

    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, IdentityResult>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UpdateUserCommandHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IdentityResult> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.Id.ToString());
            if (user == null)
                return IdentityResult.Failed(new IdentityError { Description = "Kullanıcı bulunamadı." });

            if (!string.IsNullOrWhiteSpace(request.FullName))
                user.FullName = request.FullName;
            if (!string.IsNullOrWhiteSpace(request.Email))
            {
                user.Email = request.Email;
                user.UserName = request.Email;
            }
            return await _userManager.UpdateAsync(user);
        }
    }
}
