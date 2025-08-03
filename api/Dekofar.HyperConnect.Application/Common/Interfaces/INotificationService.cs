using System;
using System.Threading;
using System.Threading.Tasks;

namespace Dekofar.HyperConnect.Application.Common.Interfaces
{
    public interface INotificationService
    {
        Task SendToUserAsync(Guid userId, string method, object? arg, CancellationToken cancellationToken = default);
    }
}
