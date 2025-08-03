using System;
using System.Threading.Tasks;

namespace Dekofar.HyperConnect.Application.Common.Interfaces
{
    public interface IActivityLogger
    {
        Task LogAsync(Guid userId, string actionType, object? data, string? ipAddress);
    }
}
