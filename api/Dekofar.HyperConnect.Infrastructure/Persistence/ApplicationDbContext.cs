using Dekofar.HyperConnect.Application.Common.Interfaces;
using Dekofar.HyperConnect.Domain.Entities;
using Dekofar.HyperConnect.Domain.Entities.Orders;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using DomainOrder = Dekofar.HyperConnect.Domain.Entities.Order;
using DomainCommission = Dekofar.HyperConnect.Domain.Entities.Commission;

namespace Dekofar.HyperConnect.Infrastructure.Persistence
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>, IApplicationDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<SupportTicket> SupportTickets => Set<SupportTicket>();
        public DbSet<SupportCategory> SupportCategories => Set<SupportCategory>();
        public DbSet<SupportCategoryRole> SupportCategoryRoles => Set<SupportCategoryRole>();
        public DbSet<ApplicationUser> Users => Set<ApplicationUser>();
        public DbSet<IdentityUserRole<Guid>> UserRoles => Set<IdentityUserRole<Guid>>();
        public DbSet<IdentityRole<Guid>> Roles => Set<IdentityRole<Guid>>();
        public DbSet<Tag> Tags { get; set; }
        public DbSet<OrderTag> OrderTags => Set<OrderTag>();
        public DbSet<ManualOrder> ManualOrders => Set<ManualOrder>();
        public DbSet<ManualOrderItem> ManualOrderItems => Set<ManualOrderItem>();
        public DbSet<OrderCommission> OrderCommissions => Set<OrderCommission>();
        public DbSet<DomainOrder> Orders => Set<DomainOrder>();
        public DbSet<DomainCommission> Commissions => Set<DomainCommission>();
        public DbSet<Discount> Discounts => Set<Discount>();
        public DbSet<Note> Notes => Set<Note>();
        public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
        public DbSet<ActivityLog> ActivityLogs => Set<ActivityLog>();
        public DbSet<UserNotification> UserNotifications => Set<UserNotification>();
        public DbSet<UserBadge> UserBadges => Set<UserBadge>();
        public DbSet<UserUIPreference> UserUIPreferences => Set<UserUIPreference>();
        public DbSet<Permission> Permissions => Set<Permission>();
        public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
        public DbSet<UserMessage> UserMessages => Set<UserMessage>();
        public DbSet<SupportTicketReply> SupportTicketReplies => Set<SupportTicketReply>();
        public DbSet<PinCoverImage> PinCoverImages => Set<PinCoverImage>();
        public DbSet<BlacklistEntry> BlacklistEntries => Set<BlacklistEntry>();
        public DbSet<CalendarTask> CalendarTasks => Set<CalendarTask>();
        public DbSet<AllowedAdminIp> AllowedAdminIps => Set<AllowedAdminIp>();
        public DbSet<DeploymentLog> DeploymentLogs => Set<DeploymentLog>();
        public DbSet<ResponseTemplate> ResponseTemplates => Set<ResponseTemplate>();
        public DbSet<ModerationRule> ModerationRules => Set<ModerationRule>();
        public DbSet<ModerationLog> ModerationLogs => Set<ModerationLog>();
        public DbSet<WorkSession> WorkSessions => Set<WorkSession>();

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
            => await base.SaveChangesAsync(cancellationToken);

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // ⚠️ Identity tabloları için ExcludeFromMigrations kaldırıldı

            builder.Entity<SupportCategory>(entity =>
            {
                entity.ToTable("SupportCategories");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(250);
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.HasMany(e => e.Roles)
                      .WithOne(r => r.Category)
                      .HasForeignKey(r => r.SupportCategoryId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<SupportCategoryRole>(entity =>
            {
                entity.ToTable("SupportCategoryRoles");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.RoleName).IsRequired().HasMaxLength(100);
            });

            builder.Entity<SupportTicket>(entity =>
            {
                entity.ToTable("SupportTickets");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).IsRequired();
                entity.Property(e => e.FilePath).HasMaxLength(500);
                entity.Property(e => e.UnreadReplyCount).HasDefaultValue(0);
                entity.HasOne(e => e.Category)
                      .WithMany(c => c.Tickets)
                      .HasForeignKey(e => e.CategoryId)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            builder.Entity<ManualOrder>(entity =>
            {
                entity.ToTable("ManualOrders");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.CustomerName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.CustomerSurname).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Phone).HasMaxLength(50);
                entity.Property(e => e.Email).HasMaxLength(100);
                entity.Property(e => e.Address).IsRequired().HasMaxLength(500);
                entity.Property(e => e.City).HasMaxLength(100);
                entity.Property(e => e.District).HasMaxLength(100);
                entity.Property(e => e.PaymentType).HasMaxLength(50);
                entity.Property(e => e.OrderNote).HasMaxLength(500);
                entity.Property(e => e.TotalAmount).HasColumnType("decimal(18,2)");
            });

            builder.Entity<Discount>(entity =>
            {
                entity.ToTable("Discounts");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Type).IsRequired();
                entity.Property(e => e.Value).HasColumnType("decimal(18,2)");
                entity.Property(e => e.IsActive).IsRequired();
                entity.Property(e => e.CreatedByUserId).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired();
            });

            builder.Entity<PinCoverImage>(entity =>
            {
                entity.ToTable("PinCoverImages");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Url).IsRequired();
            });

            builder.Entity<OrderCommission>(entity =>
            {
                entity.ToTable("OrderCommissions");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.CommissionRate).HasColumnType("decimal(18,4)");
                entity.Property(e => e.EarnedAmount).HasColumnType("decimal(18,2)");
                entity.Property(e => e.CreatedAt).IsRequired();
            });

            builder.Entity<DomainOrder>(entity =>
            {
                entity.ToTable("Orders");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.TotalAmount).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Status).HasConversion<int>();
                entity.HasOne(e => e.Seller)
                      .WithMany(u => u.Orders)
                      .HasForeignKey(e => e.SellerId)
                      .OnDelete(DeleteBehavior.SetNull);
                entity.HasOne(e => e.Customer)
                      .WithMany()
                      .HasForeignKey(e => e.CustomerId)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            builder.Entity<DomainCommission>(entity =>
            {
                entity.ToTable("Commissions");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");
                entity.HasOne(c => c.User)
                      .WithMany(u => u.Commissions)
                      .HasForeignKey(c => c.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(c => c.Order)
                      .WithMany()
                      .HasForeignKey(c => c.OrderId)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            builder.Entity<Note>(entity =>
            {
                entity.ToTable("Notes");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.TargetType).IsRequired();
                entity.Property(e => e.Text).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired();
            });

            builder.Entity<AuditLog>(entity =>
            {
                entity.ToTable("AuditLogs");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Action).IsRequired();
                entity.Property(e => e.TargetType).IsRequired();
                entity.Property(e => e.Description);
                entity.Property(e => e.Timestamp).IsRequired();
            });

            builder.Entity<Permission>(entity =>
            {
                entity.ToTable("Permissions");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(250);
            });

            builder.Entity<RolePermission>(entity =>
            {
                entity.ToTable("RolePermissions");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.RoleName).IsRequired().HasMaxLength(100);
                entity.HasOne(rp => rp.Permission)
                      .WithMany(p => p.RolePermissions)
                      .HasForeignKey(rp => rp.PermissionId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<UserMessage>(entity =>
            {
                entity.ToTable("UserMessages");
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.Sender)
                      .WithMany()
                      .HasForeignKey(e => e.SenderId)
                      .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Receiver)
                      .WithMany()
                      .HasForeignKey(e => e.ReceiverId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<SupportTicketReply>(entity =>
            {
                entity.ToTable("SupportTicketReplies");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Message).IsRequired();
                entity.HasOne(e => e.Ticket)
                      .WithMany()
                      .HasForeignKey(e => e.TicketId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.User)
                      .WithMany()
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<UserNotification>(entity =>
            {
                entity.ToTable("UserNotifications");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Type).HasMaxLength(50);
                entity.Property(e => e.CreatedAt).IsRequired();
            });

            builder.Entity<UserBadge>(entity =>
            {
                entity.ToTable("UserBadges");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Badge).IsRequired().HasMaxLength(100);
                entity.Property(e => e.AwardedAt).IsRequired();
            });

            builder.Entity<UserUIPreference>(entity =>
            {
                entity.ToTable("UserUIPreferences");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ModuleKey).IsRequired().HasMaxLength(100);
                entity.Property(e => e.PreferenceJson).IsRequired();
                entity.Property(e => e.UpdatedAt).IsRequired();
            });

            builder.Entity<ActivityLog>(entity =>
            {
                entity.ToTable("ActivityLogs");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ActionType).IsRequired().HasMaxLength(100);
                entity.Property(e => e.CreatedAt).IsRequired();
            });

            builder.Entity<ResponseTemplate>(entity =>
            {
                entity.ToTable("ResponseTemplates");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Body).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.Property(e => e.ModuleScope).HasMaxLength(100);
            });

            builder.Entity<ModerationRule>(entity =>
            {
                entity.ToTable("ModerationRules");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Pattern).IsRequired();
                entity.Property(e => e.Description).HasMaxLength(250);
            });

            builder.Entity<ModerationLog>(entity =>
            {
                entity.ToTable("ModerationLogs");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Content).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired();
            });
        }
    }
}
