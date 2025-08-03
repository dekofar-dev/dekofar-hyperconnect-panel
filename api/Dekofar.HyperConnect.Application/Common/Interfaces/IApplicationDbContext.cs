using Dekofar.HyperConnect.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace Dekofar.HyperConnect.Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<SupportTicket> SupportTickets { get; }
        DbSet<SupportCategory> SupportCategories { get; }
        DbSet<SupportCategoryRole> SupportCategoryRoles { get; }
        DbSet<ManualOrder> ManualOrders { get; }
        DbSet<ManualOrderItem> ManualOrderItems { get; }
        DbSet<OrderCommission> OrderCommissions { get; }
        DbSet<Order> Orders { get; }
        DbSet<Commission> Commissions { get; }
        DbSet<Discount> Discounts { get; }
        DbSet<Note> Notes { get; }
        DbSet<AuditLog> AuditLogs { get; }
        DbSet<ActivityLog> ActivityLogs { get; }
        DbSet<UserNotification> UserNotifications { get; }
        DbSet<UserBadge> UserBadges { get; }
        DbSet<UserUIPreference> UserUIPreferences { get; }
        DbSet<ApplicationUser> Users { get; }
        DbSet<IdentityUserRole<Guid>> UserRoles { get; }
        DbSet<IdentityRole<Guid>> Roles { get; }
        DbSet<Permission> Permissions { get; }
        DbSet<RolePermission> RolePermissions { get; }
        DbSet<UserMessage> UserMessages { get; }
        DbSet<SupportTicketReply> SupportTicketReplies { get; }
        DbSet<PinCoverImage> PinCoverImages { get; }
        DbSet<ResponseTemplate> ResponseTemplates { get; }
        DbSet<ModerationRule> ModerationRules { get; }
        DbSet<ModerationLog> ModerationLogs { get; }
        /// <summary>
        /// Kullanıcı çalışma oturumları tablosu
        /// </summary>
        DbSet<WorkSession> WorkSessions { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
