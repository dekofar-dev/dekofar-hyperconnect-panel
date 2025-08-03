using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dekofar.HyperConnect.Application.Common.Interfaces;
using Dekofar.HyperConnect.Application.Dashboard.DTOs;
using Dekofar.HyperConnect.Application.Interfaces;
using Dekofar.HyperConnect.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Dekofar.HyperConnect.Application.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IApplicationDbContext _context;

        public DashboardService(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardSummaryDto> GetSummaryAsync()
        {
            var now = DateTime.UtcNow;
            var activeThreshold = now.AddDays(-30);

            var totalUsersTask = _context.Users.CountAsync();
            var activeUsersTask = _context.Users.CountAsync(u => u.LastSeen != null && u.LastSeen >= activeThreshold);
            var orderSalesTask = _context.Orders.SumAsync(o => (decimal?)o.TotalAmount) ;
            var manualSalesTask = _context.ManualOrders.SumAsync(o => (decimal?)o.TotalAmount) ;
            var orderCountTask = _context.Orders.CountAsync();
            var manualOrderCountTask = _context.ManualOrders.CountAsync();
            var totalSupportTicketsTask = _context.SupportTickets.CountAsync();
            var openTicketsTask = _context.SupportTickets.CountAsync(t => t.Status != SupportTicketStatus.Closed);
            var commissionPaidTask = _context.Commissions.SumAsync(c => (decimal?)c.Amount);

            await Task.WhenAll(totalUsersTask, activeUsersTask, orderSalesTask, manualSalesTask,
                orderCountTask, manualOrderCountTask, totalSupportTicketsTask, openTicketsTask, commissionPaidTask);

            var topSellers = await _context.Orders
                .Where(o => o.SellerId != null)
                .Include(o => o.Seller)
                .GroupBy(o => new { o.SellerId, o.Seller.FullName })
                .Select(g => new TopSellerDto
                {
                    UserId = g.Key.SellerId!.Value,
                    FullName = g.Key.FullName,
                    TotalSales = g.Sum(o => o.TotalAmount)
                })
                .OrderByDescending(x => x.TotalSales)
                .Take(5)
                .ToListAsync();

            return new DashboardSummaryDto
            {
                TotalUsers = totalUsersTask.Result,
                ActiveUsers = activeUsersTask.Result,
                TotalSales = (orderSalesTask.Result ?? 0) + (manualSalesTask.Result ?? 0),
                TotalOrders = orderCountTask.Result + manualOrderCountTask.Result,
                TotalSupportTickets = totalSupportTicketsTask.Result,
                OpenTickets = openTicketsTask.Result,
                TotalCommissionPaid = commissionPaidTask.Result ?? 0,
                TopSellers = topSellers
            };
        }

        public async Task<List<SalesOverTimeDto>> GetSalesOverTimeAsync(int days)
        {
            var startDate = DateTime.UtcNow.Date.AddDays(-days + 1);

            var orderSalesTask = _context.Orders
                .Where(o => o.CreatedAt >= startDate)
                .GroupBy(o => o.CreatedAt.Date)
                .Select(g => new { Date = g.Key, Total = g.Sum(x => x.TotalAmount) })
                .ToListAsync();

            var manualSalesTask = _context.ManualOrders
                .Where(o => o.CreatedAt >= startDate)
                .GroupBy(o => o.CreatedAt.Date)
                .Select(g => new { Date = g.Key, Total = g.Sum(x => x.TotalAmount) })
                .ToListAsync();

            await Task.WhenAll(orderSalesTask, manualSalesTask);

            var dict = orderSalesTask.Result.Concat(manualSalesTask.Result)
                .GroupBy(x => x.Date)
                .ToDictionary(g => g.Key, g => g.Sum(x => x.Total));

            var result = new List<SalesOverTimeDto>();
            for (int i = 0; i < days; i++)
            {
                var date = startDate.AddDays(i);
                dict.TryGetValue(date, out var total);
                result.Add(new SalesOverTimeDto { Date = date, TotalSales = total });
            }

            return result;
        }

        public async Task<List<TopProductDto>> GetTopProductsAsync(int limit)
        {
            var products = await _context.ManualOrderItems
                .GroupBy(i => i.ProductName)
                .Select(g => new TopProductDto
                {
                    ProductName = g.Key,
                    UnitsSold = g.Sum(i => i.Quantity),
                    Revenue = g.Sum(i => i.Total)
                })
                .OrderByDescending(p => p.UnitsSold)
                .Take(limit)
                .ToListAsync();

            return products;
        }

        public async Task<List<TicketActivityDto>> GetTicketActivityAsync(int days)
        {
            var startDate = DateTime.UtcNow.Date.AddDays(-days + 1);

            var createdTask = _context.SupportTickets
                .Where(t => t.CreatedAt >= startDate)
                .GroupBy(t => t.CreatedAt.Date)
                .Select(g => new { Date = g.Key, Count = g.Count() })
                .ToListAsync();

            var resolvedTask = _context.SupportTickets
                .Where(t => t.Status == SupportTicketStatus.Closed && t.LastUpdatedAt >= startDate)
                .GroupBy(t => t.LastUpdatedAt.Date)
                .Select(g => new { Date = g.Key, Count = g.Count() })
                .ToListAsync();

            await Task.WhenAll(createdTask, resolvedTask);

            var createdDict = createdTask.Result.ToDictionary(x => x.Date, x => x.Count);
            var resolvedDict = resolvedTask.Result.ToDictionary(x => x.Date, x => x.Count);

            var result = new List<TicketActivityDto>();
            for (int i = 0; i < days; i++)
            {
                var date = startDate.AddDays(i);
                createdDict.TryGetValue(date, out var created);
                resolvedDict.TryGetValue(date, out var resolved);
                result.Add(new TicketActivityDto { Date = date, Created = created, Resolved = resolved });
            }

            return result;
        }

        public Task<int> GetTotalUsersAsync()
        {
            return _context.Users.CountAsync();
        }

        public async Task<int> GetTotalOrdersAsync()
        {
            var orderCount = await _context.Orders.CountAsync();
            var manualCount = await _context.ManualOrders.CountAsync();
            return orderCount + manualCount;
        }

        public Task<int> GetTotalSupportTicketsAsync()
        {
            return _context.SupportTickets.CountAsync();
        }

        /// <summary>
        /// Aggregates commission payouts by month for the last <paramref name="months"/> months.
        /// Used by the admin dashboard to build earnings charts.
        /// </summary>
        public async Task<List<MonthlyCommissionDto>> GetMonthlyCommissionsAsync(int months)
        {
            var start = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1).AddMonths(-months + 1);
            return await _context.Commissions
                .Where(c => c.CreatedAt >= start)
                .GroupBy(c => new { c.CreatedAt.Year, c.CreatedAt.Month })
                .Select(g => new MonthlyCommissionDto
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Total = g.Sum(x => x.Amount)
                })
                .OrderBy(g => g.Year).ThenBy(g => g.Month)
                .ToListAsync();
        }
    }
}
