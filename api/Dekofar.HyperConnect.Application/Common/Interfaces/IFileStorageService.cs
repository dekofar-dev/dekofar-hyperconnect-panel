using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Dekofar.HyperConnect.Application.Common.Interfaces
{
    public interface IFileStorageService
    {
        Task<string> SaveProfileImageAsync(IFormFile file, Guid userId);
        Task<string> SaveChatAttachmentAsync(IFormFile file, Guid userId);
    }
}
