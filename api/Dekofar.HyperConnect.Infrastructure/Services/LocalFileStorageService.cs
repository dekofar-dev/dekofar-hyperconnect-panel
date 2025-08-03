using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Dekofar.HyperConnect.Application.Common.Interfaces;

namespace Dekofar.HyperConnect.Infrastructure.Services
{
    public class LocalFileStorageService : IFileStorageService
    {
        public async Task<string> SaveProfileImageAsync(IFormFile file, Guid userId)
        {
            var uploadsRoot = Path.Combine(Directory.GetCurrentDirectory(), "uploads", "avatars");
            Directory.CreateDirectory(uploadsRoot);

            var filePath = Path.Combine(uploadsRoot, $"{userId}.jpg");
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"/uploads/avatars/{userId}.jpg";
        }

        public async Task<string> SaveChatAttachmentAsync(IFormFile file, Guid userId)
        {
            var uploadsRoot = Path.Combine(Directory.GetCurrentDirectory(), "uploads", "chat", userId.ToString());
            Directory.CreateDirectory(uploadsRoot);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(uploadsRoot, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"/uploads/chat/{userId}/{fileName}";
        }
    }
}
