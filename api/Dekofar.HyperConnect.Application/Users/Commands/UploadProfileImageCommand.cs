using System;
using System.Threading;
using System.Threading.Tasks;
using Dekofar.HyperConnect.Domain.Entities;
using Dekofar.HyperConnect.Application.Common.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Dekofar.HyperConnect.Application.Users.Commands
{
    public class UploadProfileImageCommand : IRequest<string>
    {
        public Guid UserId { get; set; }
        public IFormFile File { get; set; } = default!;
    }

    public class UploadProfileImageCommandHandler : IRequestHandler<UploadProfileImageCommand, string>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IFileStorageService _fileStorageService;

        public UploadProfileImageCommandHandler(UserManager<ApplicationUser> userManager, IFileStorageService fileStorageService)
        {
            _userManager = userManager;
            _fileStorageService = fileStorageService;
        }

        public async Task<string> Handle(UploadProfileImageCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user == null)
            {
                throw new Exception("User not found");
            }

            var url = await _fileStorageService.SaveProfileImageAsync(request.File, request.UserId);
            user.AvatarUrl = url;
            await _userManager.UpdateAsync(user);
            return url;
        }
    }
}
