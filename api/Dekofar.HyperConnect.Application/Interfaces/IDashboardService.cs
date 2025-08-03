using System.Collections.Generic;
using System.Threading.Tasks;
using Dekofar.HyperConnect.Application.Dashboard.DTOs;

namespace Dekofar.HyperConnect.Application.Interfaces
{
    public interface IDashboardService
    {
        Task<DashboardSummaryDto> GetSummaryAsync();
        Task<List<SalesOverTimeDto>> GetSalesOverTimeAsync(int days);
        Task<List<TopProductDto>> GetTopProductsAsync(int limit);
        Task<List<TicketActivityDto>> GetTicketActivityAsync(int days);
        Task<int> GetTotalUsersAsync();
        Task<int> GetTotalOrdersAsync();
        Task<int> GetTotalSupportTicketsAsync();
        Task<List<MonthlyCommissionDto>> GetMonthlyCommissionsAsync(int months);
    }
}
