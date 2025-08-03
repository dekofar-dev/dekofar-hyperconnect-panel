using System;
using System.Threading.Tasks;
using Dekofar.HyperConnect.Domain.Entities;

namespace Dekofar.HyperConnect.Application.Interfaces
{
    public class ModerationResult
    {
        public bool Blocked { get; set; }
        public ModerationRule? Rule { get; set; }
    }

    public interface IModerationService
    {
        Task<ModerationResult> CheckAsync(string? content, Guid? userId);
    }
}
