using System;
using System.Threading.Tasks;
using Dekofar.API.Authorization;
using Dekofar.HyperConnect.Application.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dekofar.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/notifications")]
    public class UserNotificationsController : ControllerBase
    {
        private readonly IUserNotificationService _service;

        public UserNotificationsController(IUserNotificationService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetMyNotifications()
        {
            var userId = User.GetUserId();
            if (userId == null) return Unauthorized();
            var list = await _service.GetLatestAsync(userId.Value);
            return Ok(list);
        }

        [HttpPost("{id:guid}/read")]
        public async Task<IActionResult> MarkAsRead(Guid id)
        {
            var userId = User.GetUserId();
            if (userId == null) return Unauthorized();
            await _service.MarkAsReadAsync(userId.Value, id);
            return Ok();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var userId = User.GetUserId();
            if (userId == null) return Unauthorized();
            await _service.DeleteAsync(userId.Value, id);
            return NoContent();
        }
    }
}
