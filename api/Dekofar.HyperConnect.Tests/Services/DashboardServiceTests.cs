using System;
using System.Threading.Tasks;
using Dekofar.HyperConnect.Application.Services;
using Dekofar.HyperConnect.Domain.Entities;
using Dekofar.HyperConnect.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Dekofar.HyperConnect.Tests.Services
{
    public class DashboardServiceTests
    {
        private ApplicationDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task GetSummaryAsync_ReturnsCorrectCounts()
        {
            using var context = CreateContext();
            context.Users.Add(new ApplicationUser { Id = Guid.NewGuid(), LastSeen = DateTime.UtcNow });
            context.Orders.Add(new Dekofar.HyperConnect.Domain.Entities.Order { Id = Guid.NewGuid(), CreatedAt = DateTime.UtcNow, TotalAmount = 100 });
            context.SupportTickets.Add(new SupportTicket { Id = Guid.NewGuid(), Title = "t", Description = "d", CreatedByUserId = Guid.NewGuid(), CreatedAt = DateTime.UtcNow, LastUpdatedAt = DateTime.UtcNow, Status = SupportTicketStatus.Open });
            context.Commissions.Add(new Commission { Id = Guid.NewGuid(), UserId = Guid.NewGuid(), Amount = 10, CreatedAt = DateTime.UtcNow });
            await context.SaveChangesAsync();

            var service = new DashboardService(context);
            var summary = await service.GetSummaryAsync();

            Assert.Equal(1, summary.TotalUsers);
            Assert.Equal(1, summary.ActiveUsers);
            Assert.Equal(100, summary.TotalSales);
            Assert.Equal(1, summary.TotalOrders);
            Assert.Equal(1, summary.TotalSupportTickets);
            Assert.Equal(1, summary.OpenTickets);
            Assert.Equal(10, summary.TotalCommissionPaid);
        }
    }
}
