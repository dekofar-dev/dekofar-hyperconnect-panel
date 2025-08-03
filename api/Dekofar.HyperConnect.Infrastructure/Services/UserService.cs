using System;
using System.Linq;
using System.Threading.Tasks;
using Dekofar.HyperConnect.Application.Interfaces;
using Dekofar.HyperConnect.Application.Users.DTOs;
using Dekofar.HyperConnect.Application.Common.Interfaces;
using Dekofar.HyperConnect.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Dekofar.HyperConnect.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly IApplicationDbContext _dbContext;

        public UserService(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<UserProfileDto?> GetProfileWithStatsAsync(Guid userId)
        {
            var user = await _dbContext.Users
                .Where(u => u.Id == userId)
                .Select(u => new UserProfileDto
                {
                    Id = u.Id,
                    FullName = u.FullName ?? string.Empty,
                    Email = u.Email!,
                    AvatarUrl = u.AvatarUrl,
                    UnreadMessageCount = _dbContext.UserMessages.Count(m => m.ReceiverId == u.Id && !m.IsRead),
                    LastMessageDate = _dbContext.UserMessages
                        .Where(m => m.SenderId == u.Id || m.ReceiverId == u.Id)
                        .OrderByDescending(m => m.SentAt)
                        .Select(m => m.SentAt)
                        .FirstOrDefault(),
                    LastSupportActivityAt = _dbContext.SupportTickets
                        .Where(t => t.CreatedByUserId == u.Id || t.AssignedUserId == u.Id)
                        .OrderByDescending(t => t.LastUpdatedAt)
                        .Select(t => t.LastUpdatedAt)
                        .FirstOrDefault(),
                    IsOnline = u.IsOnline
                })
                .FirstOrDefaultAsync();

            return user;
        }

        public async Task<ProfileSummaryDto?> GetProfileSummaryAsync(Guid userId)
        {
            var summary = await _dbContext.Users
                .Where(u => u.Id == userId)
                .Select(u => new ProfileSummaryDto
                {
                    FullName = u.FullName ?? string.Empty,
                    AvatarUrl = u.AvatarUrl,
                    JoinedAt = u.MembershipDate,
                    TotalSupportTickets = _dbContext.SupportTickets.Count(t => t.CreatedByUserId == u.Id),
                    OpenSupportTickets = _dbContext.SupportTickets.Count(t => t.CreatedByUserId == u.Id && t.Status != SupportTicketStatus.Closed),
                    ClosedSupportTickets = _dbContext.SupportTickets.Count(t => t.CreatedByUserId == u.Id && t.Status == SupportTicketStatus.Closed),
                    LastSupportActivityAt = _dbContext.SupportTickets
                        .Where(t => t.CreatedByUserId == u.Id)
                        .OrderByDescending(t => t.LastUpdatedAt)
                        .Select(t => (DateTime?)t.LastUpdatedAt)
                        .FirstOrDefault(),
                    TotalMessagesSent = _dbContext.UserMessages.Count(m => m.SenderId == u.Id),
                    UnreadMessageCount = _dbContext.UserMessages.Count(m => m.ReceiverId == u.Id && !m.IsRead),
                    LastMessageDate = _dbContext.UserMessages
                        .Where(m => m.SenderId == u.Id || m.ReceiverId == u.Id)
                        .OrderByDescending(m => m.SentAt)
                        .Select(m => (DateTime?)m.SentAt)
                        .FirstOrDefault(),
                    TotalSalesAmount = _dbContext.ManualOrders
                        .Where(o => o.CreatedByUserId == u.Id)
                        .Sum(o => (decimal?)o.TotalAmount) ?? 0,
                    TotalCommission = _dbContext.OrderCommissions
                        .Where(c => c.UserId == u.Id)
                        .Sum(c => (decimal?)c.EarnedAmount) ?? 0,
                    Roles = (from ur in _dbContext.UserRoles
                             join r in _dbContext.Roles on ur.RoleId equals r.Id
                             where ur.UserId == u.Id
                             select r.Name!).ToArray()
                })
                .FirstOrDefaultAsync();

            return summary;
        }

        public async Task<SalesSummaryDto?> GetSalesSummaryAsync(Guid userId)
        {
            var summary = await _dbContext.Users
                .Where(u => u.Id == userId)
                .Select(u => new SalesSummaryDto
                {
                    TotalSalesAmount = _dbContext.Orders
                        .Where(o => o.SellerId == u.Id)
                        .Sum(o => (decimal?)o.TotalAmount) ?? 0,
                    TotalCommissionAmount = _dbContext.Commissions
                        .Where(c => c.UserId == u.Id)
                        .Sum(c => (decimal?)c.Amount) ?? 0,
                    OrdersCount = _dbContext.Orders.Count(o => o.SellerId == u.Id),
                    CommissionCount = _dbContext.Commissions.Count(c => c.UserId == u.Id)
                })
                .FirstOrDefaultAsync();

            return summary;
        }
    }
}
