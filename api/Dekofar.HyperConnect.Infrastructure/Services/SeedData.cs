using System;
using System.Linq;
using System.Threading.Tasks;
using Dekofar.HyperConnect.Domain.Entities;
using Dekofar.HyperConnect.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Dekofar.HyperConnect.Infrastructure.Services
{
    public static class SeedData
    {
        public static async Task SeedDefaultsAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            string[] roles = new[] { "Admin", "Support", "Warehouse", "Returns", "Finance" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole<Guid>(role));
                }
            }

            var adminEmail = "admin@dekofar.com";
            var adminPassword = "AdminRecep123*";

            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FullName = "Admin User",
                    MembershipDate = DateTime.UtcNow,
                    IsOnline = false,
                    LastSeen = DateTime.UtcNow
                };
                var createUserResult = await userManager.CreateAsync(adminUser, adminPassword);
                if (!createUserResult.Succeeded)
                {
                    return;
                }
            }

            if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }

            // Seed a default support staff member
            var supportEmail = "support@dekofar.com";
            var supportUser = await userManager.FindByEmailAsync(supportEmail);
            if (supportUser == null)
            {
                supportUser = new ApplicationUser
                {
                    UserName = supportEmail,
                    Email = supportEmail,
                    FullName = "Support Agent",
                    MembershipDate = DateTime.UtcNow.AddDays(-7),
                    IsOnline = false,
                    LastSeen = DateTime.UtcNow.AddMinutes(-30)
                };
                await userManager.CreateAsync(supportUser, "Support123*");
            }
            if (!await userManager.IsInRoleAsync(supportUser, "Support"))
            {
                await userManager.AddToRoleAsync(supportUser, "Support");
            }

            // Seed a demo customer used for messaging examples
            var customerEmail = "customer@dekofar.com";
            var demoUser = await userManager.FindByEmailAsync(customerEmail);
            if (demoUser == null)
            {
                demoUser = new ApplicationUser
                {
                    UserName = customerEmail,
                    Email = customerEmail,
                    FullName = "Demo Customer",
                    MembershipDate = DateTime.UtcNow.AddMonths(-1),
                    IsOnline = false,
                    LastSeen = DateTime.UtcNow.AddHours(-1)
                };
                await userManager.CreateAsync(demoUser, "Customer123*");
            }

            var generalCategory = await context.SupportCategories.FirstOrDefaultAsync();
            if (generalCategory == null)
            {
                generalCategory = new SupportCategory
                {
                    Id = Guid.NewGuid(),
                    Name = "General",
                    Description = "General support",
                    CreatedAt = DateTime.UtcNow
                };
                context.SupportCategories.Add(generalCategory);
                await context.SaveChangesAsync();
            }

            if (!await context.SupportTickets.AnyAsync())
            {
                var openTicket = new SupportTicket
                {
                    Id = Guid.NewGuid(),
                    Title = "Account question",
                    Description = "Customer has a question about their account",
                    CreatedByUserId = demoUser.Id,
                    CategoryId = generalCategory.Id,
                    Status = SupportTicketStatus.Open,
                    Priority = SupportTicketPriority.Medium,
                    CreatedAt = DateTime.UtcNow.AddDays(-2),
                    LastUpdatedAt = DateTime.UtcNow.AddDays(-2),
                    UnreadReplyCount = 0
                };

                var inProgressTicket = new SupportTicket
                {
                    Id = Guid.NewGuid(),
                    Title = "Payment issue",
                    Description = "Payment failed for order #123",
                    CreatedByUserId = demoUser.Id,
                    AssignedUserId = supportUser.Id,
                    CategoryId = generalCategory.Id,
                    Status = SupportTicketStatus.InProgress,
                    Priority = SupportTicketPriority.High,
                    CreatedAt = DateTime.UtcNow.AddDays(-1),
                    LastUpdatedAt = DateTime.UtcNow.AddHours(-12),
                    UnreadReplyCount = 1
                };

                var closedTicket = new SupportTicket
                {
                    Id = Guid.NewGuid(),
                    Title = "Refund request",
                    Description = "Customer requested a refund",
                    CreatedByUserId = demoUser.Id,
                    CategoryId = generalCategory.Id,
                    Status = SupportTicketStatus.Closed,
                    Priority = SupportTicketPriority.Low,
                    CreatedAt = DateTime.UtcNow.AddDays(-10),
                    LastUpdatedAt = DateTime.UtcNow.AddDays(-8),
                    UnreadReplyCount = 0
                };

                context.SupportTickets.AddRange(openTicket, inProgressTicket, closedTicket);
                await context.SaveChangesAsync();
            }

            // Seed example user-to-user messages
            if (!await context.UserMessages.AnyAsync())
            {
                var m1 = new UserMessage
                {
                    Id = Guid.NewGuid(),
                    SenderId = demoUser.Id,
                    ReceiverId = adminUser.Id,
                    Text = "Hi admin, I need help",
                    SentAt = DateTime.UtcNow.AddMinutes(-90),
                    IsRead = false
                };

                var m2 = new UserMessage
                {
                    Id = Guid.NewGuid(),
                    SenderId = adminUser.Id,
                    ReceiverId = demoUser.Id,
                    Text = "Sure, how can I assist?",
                    FileUrl = "https://example.com/manual.pdf",
                    FileType = "application/pdf",
                    FileSize = 1024,
                    SentAt = DateTime.UtcNow.AddMinutes(-80),
                    IsRead = true,
                    ReadAt = DateTime.UtcNow.AddMinutes(-70)
                };

                context.UserMessages.AddRange(m1, m2);
                await context.SaveChangesAsync();

                // Update unread message counters for the users
                demoUser.UnreadMessageCount = await context.UserMessages.CountAsync(m => m.ReceiverId == demoUser.Id && !m.IsRead);
                adminUser.UnreadMessageCount = await context.UserMessages.CountAsync(m => m.ReceiverId == adminUser.Id && !m.IsRead);
                context.Users.Update(demoUser);
                context.Users.Update(adminUser);
                await context.SaveChangesAsync();
            }

            // Seed sample commissions for reporting
            if (!await context.Commissions.AnyAsync())
            {
                context.Commissions.AddRange(
                    new Commission
                    {
                        Id = Guid.NewGuid(),
                        UserId = demoUser.Id,
                        Amount = 75m,
                        CreatedAt = DateTime.UtcNow.AddMonths(-1),
                        Description = "January commission"
                    },
                    new Commission
                    {
                        Id = Guid.NewGuid(),
                        UserId = demoUser.Id,
                        Amount = 50m,
                        CreatedAt = DateTime.UtcNow.AddMonths(-2),
                        Description = "December commission"
                    }
                );
                await context.SaveChangesAsync();
            }

            // Seed default permissions used by policy-based authorization
            var defaultPermissions = new[]
            {
                new Permission { Id = Guid.NewGuid(), Name = "CanAssignTicket", Description = "Assign support tickets" },
                new Permission { Id = Guid.NewGuid(), Name = "CanManageDiscounts", Description = "Manage discounts" },
                new Permission { Id = Guid.NewGuid(), Name = "CanEditDueDate", Description = "Edit ticket due dates" }
            };

            foreach (var perm in defaultPermissions)
            {
                if (!await context.Permissions.AnyAsync(p => p.Name == perm.Name))
                {
                    context.Permissions.Add(perm);
                }
            }
            await context.SaveChangesAsync();

            var adminRoleName = "Admin";
            var adminPermissions = await context.Permissions.ToListAsync();

            // Ensure the Admin role has all default permissions
            foreach (var perm in adminPermissions)
            {
                if (!await context.RolePermissions.AnyAsync(rp => rp.RoleName == adminRoleName && rp.PermissionId == perm.Id))
                {
                    context.RolePermissions.Add(new RolePermission
                    {
                        Id = Guid.NewGuid(),
                        RoleName = adminRoleName,
                        PermissionId = perm.Id
                    });
                }
            }
            await context.SaveChangesAsync();
        }
    }
}
