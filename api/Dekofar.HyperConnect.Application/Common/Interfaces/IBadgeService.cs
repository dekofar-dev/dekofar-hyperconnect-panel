using System;
using System.Threading.Tasks;

namespace Dekofar.HyperConnect.Application.Common.Interfaces
{
    public interface IBadgeService
    {
        Task EvaluateAsync(Guid userId);
    }
}
