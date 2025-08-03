using System.Collections.Generic;
using System.Threading.Tasks;
using Dekofar.HyperConnect.Application.Dashboard.DTOs;
using Dekofar.HyperConnect.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Dekofar.API.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/dashboard")]
    [Authorize(Roles = "Admin")]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        /// <summary>
        /// Gets summary metrics for the dashboard.
        /// </summary>
        [HttpGet("summary")]
        [ProducesResponseType(typeof(DashboardSummaryDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetSummary()
        {
            var summary = await _dashboardService.GetSummaryAsync();
            return Ok(summary);
        }

        /// <summary>
        /// Gets sales totals grouped by date within a range.
        /// </summary>
        [HttpGet("sales-over-time")]
        [ProducesResponseType(typeof(List<SalesOverTimeDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetSalesOverTime([FromQuery] string range = "7d")
        {
            var days = range switch
            {
                "30d" => 30,
                "90d" => 90,
                _ => 7
            };
            var data = await _dashboardService.GetSalesOverTimeAsync(days);
            return Ok(data);
        }

        /// <summary>
        /// Gets the most sold products.
        /// </summary>
        [HttpGet("top-products")]
        [ProducesResponseType(typeof(List<TopProductDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTopProducts([FromQuery] int limit = 5)
        {
            var data = await _dashboardService.GetTopProductsAsync(limit);
            return Ok(data);
        }

        /// <summary>
        /// Gets support ticket activity over time.
        /// </summary>
        [HttpGet("ticket-activity")]
        [ProducesResponseType(typeof(List<TicketActivityDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTicketActivity([FromQuery] string range = "30d")
        {
            var days = range switch
            {
                "7d" => 7,
                "90d" => 90,
                _ => 30
            };
            var data = await _dashboardService.GetTicketActivityAsync(days);
            return Ok(data);
        }
    }
}
