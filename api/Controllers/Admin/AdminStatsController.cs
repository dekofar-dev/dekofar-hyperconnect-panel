using System.Threading.Tasks;
using Dekofar.HyperConnect.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dekofar.API.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/stats")]
    // Only administrators can access these endpoints via role-based authorization
    [Authorize(Roles = "Admin")]
    public class AdminStatsController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public AdminStatsController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        /// <summary>
        /// Returns the total number of registered users.
        /// </summary>
        [HttpGet("total-users")]
        public async Task<IActionResult> GetTotalUsers()
        {
            var total = await _dashboardService.GetTotalUsersAsync();
            return Ok(total);
        }

        /// <summary>
        /// Returns the total number of orders (manual and synced).
        /// </summary>
        [HttpGet("total-orders")]
        public async Task<IActionResult> GetTotalOrders()
        {
            var total = await _dashboardService.GetTotalOrdersAsync();
            return Ok(total);
        }

        /// <summary>
        /// Returns the total number of support tickets.
        /// </summary>
        [HttpGet("total-support-tickets")]
        public async Task<IActionResult> GetTotalSupportTickets()
        {
            var total = await _dashboardService.GetTotalSupportTicketsAsync();
            return Ok(total);
        }

        /// <summary>
        /// Returns commission totals grouped by month for the requested period.
        /// </summary>
        [HttpGet("monthly-commissions")]
        public async Task<IActionResult> GetMonthlyCommissions([FromQuery] int months = 6)
        {
            var data = await _dashboardService.GetMonthlyCommissionsAsync(months);
            return Ok(data);
        }
    }
}
