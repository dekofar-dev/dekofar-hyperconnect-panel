using System;
using System.Threading.Tasks;
using Dekofar.HyperConnect.Application.Users.DTOs;

namespace Dekofar.HyperConnect.Application.Interfaces
{
    public interface IUserService
    {
        Task<UserProfileDto?> GetProfileWithStatsAsync(Guid userId);
        Task<ProfileSummaryDto?> GetProfileSummaryAsync(Guid userId);
        Task<SalesSummaryDto?> GetSalesSummaryAsync(Guid userId);
    }
}
