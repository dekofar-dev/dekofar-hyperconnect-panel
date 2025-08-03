using Dekofar.HyperConnect.Application.Common.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dekofar.API.Controllers.Admin
{
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class BulkActionsController : ControllerBase
    {
        [HttpPost("api/orders/bulk-update-status")]
        public IActionResult BulkUpdateOrders([FromBody] BulkActionDto dto)
        {
            // TODO: implement order bulk update logic
            return Ok(new { processed = dto.Ids.Count, action = dto.Action });
        }

        [HttpPost("api/returns/bulk-assign-admin")]
        public IActionResult BulkAssignReturns([FromBody] BulkActionDto dto)
        {
            // TODO: implement returns bulk assign logic
            return Ok(new { processed = dto.Ids.Count, admin = dto.AdminId });
        }
    }
}
