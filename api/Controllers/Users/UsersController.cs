using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Dekofar.HyperConnect.Application.Users.Commands;
using Dekofar.HyperConnect.Application.Users.DTOs;
using Dekofar.HyperConnect.Application.Users.Queries;
using Dekofar.HyperConnect.Application.Interfaces;
using Dekofar.API.Authorization;
using Dekofar.HyperConnect.Domain.Entities;
using Dekofar.HyperConnect.Application.Common.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Dekofar.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    // Kullanıcı yönetimi ile ilgili işlemleri sağlayan controller
    public class UsersController : ControllerBase
    {
        // MediatR aracısı
        private readonly IMediator _mediator;
        private readonly IUserService _userService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IActivityLogger _activityLogger;
        private readonly IWorkSessionService _workSessionService;
        private readonly INotificationService _notificationService;

        // MediatR bağımlılığını alan kurucu
        public UsersController(IMediator mediator, IUserService userService, UserManager<ApplicationUser> userManager, IActivityLogger activityLogger, IWorkSessionService workSessionService, INotificationService notificationService)
        {
            _mediator = mediator;
            _userService = userService;
            _userManager = userManager;
            _activityLogger = activityLogger;
            _workSessionService = workSessionService;
            _notificationService = notificationService;
        }

        // Sistemdeki tüm kullanıcıları döner
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<UserDto>>> GetAllUsers()
        {
            var users = await _mediator.Send(new GetAllUsersWithRolesQuery());
            return Ok(users);
        }

        // Bu endpoint mevcut kullanıcının profilini getirir.
        // Erişim: Giriş yapmış tüm kullanıcılar.
        // Çıktı: Kullanıcı bilgileri
        [HttpGet("me")]
        [Authorize]
        public async Task<ActionResult<UserDto?>> GetMe()
        {
            var userId = User.GetUserId();
            if (userId == null) return Unauthorized();
            var user = await _mediator.Send(new GetUserProfileQuery { UserId = userId.Value });
            if (user == null) return NotFound();
            return Ok(user);
        }

        // Kullanıcıya rol ataması yapar
        [HttpPost("{id:guid}/roles")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AssignRoles(Guid id, [FromBody] List<string> roles)
        {
            var result = await _mediator.Send(new AssignRolesToUserCommand { UserId = id, Roles = roles });
            if (result.Succeeded)
            {
                return Ok();
            }
            return BadRequest(result.Errors);
        }

        // Bu endpoint kullanıcı profilini günceller.
        // Erişim: Kullanıcının kendisi veya admin.
        // Girdi: FullName ve Email
        [HttpPut("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserCommand command)
        {
            var currentUserId = User.GetUserId();
            if (currentUserId == null) return Unauthorized();
            if (currentUserId != id && !User.IsInRole("Admin")) return Forbid();
            command.Id = id;
            var result = await _mediator.Send(command);
            if (result.Succeeded) return Ok();
            return BadRequest(result.Errors);
        }

        // Kullanıcının profil resmini yükler
        [HttpPost("{id:guid}/avatar")]
        [Authorize]
        public async Task<IActionResult> UploadAvatar(Guid id, [FromForm] IFormFile file)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (currentUserId == null || currentUserId != id.ToString())
            {
                return Forbid();
            }

            var url = await _mediator.Send(new UploadProfileImageCommand { UserId = id, File = file });
            return Ok(new { avatarUrl = url });
        }

        // Bu endpoint profil resmini siler.
        // Erişim: Kullanıcının kendisi veya admin.
        // Girdi: Yok
        [HttpDelete("{id:guid}/avatar")]
        [Authorize]
        public async Task<IActionResult> RemoveAvatar(Guid id)
        {
            var currentUserId = User.GetUserId();
            if (currentUserId == null) return Unauthorized();
            if (currentUserId != id && !User.IsInRole("Admin")) return Forbid();
            var result = await _mediator.Send(new RemoveProfileImageCommand { UserId = id });
            if (result.Succeeded) return Ok();
            return BadRequest(result.Errors);
        }

        // Bu endpoint kullanıcı şifresini günceller.
        // Erişim: Kullanıcının kendisi.
        // Girdi: Mevcut şifre ve yeni şifre
        [HttpPost("{id:guid}/change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword(Guid id, [FromBody] ChangePasswordCommand command)
        {
            var currentUserId = User.GetUserId();
            if (currentUserId == null || currentUserId != id) return Forbid();
            command.UserId = id;
            var result = await _mediator.Send(command);
            if (result.Succeeded) return Ok();
            return BadRequest(result.Errors);
        }

        // Bu endpoint admin tarafından şifre sıfırlamak için kullanılır.
        // Erişim: Admin
        // Girdi: Yeni şifre
        [HttpPost("{id:guid}/reset-password")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ResetPassword(Guid id, [FromBody] ResetPasswordCommand command)
        {
            command.UserId = id;
            var result = await _mediator.Send(command);
            if (result.Succeeded) return Ok();
            return BadRequest(result.Errors);
        }

        [HttpGet("me/stats")]
        [Authorize]
        public async Task<IActionResult> GetMyProfileWithStats()
        {
            var userId = User.GetUserId();
            if (userId == null) return Unauthorized();
            var profile = await _userService.GetProfileWithStatsAsync(userId.Value);
            return Ok(profile);
        }

        [HttpGet("me/summary")]
        [Authorize]
        public async Task<ActionResult<ProfileSummaryDto>> GetProfileSummary()
        {
            var userId = User.GetUserId();
            if (userId == null) return Unauthorized();
            var summary = await _userService.GetProfileSummaryAsync(userId.Value);
            if (summary == null) return NotFound();
            return Ok(summary);
        }

        [HttpGet("{id:guid}/sales-summary")]
        [Authorize]
        public async Task<ActionResult<SalesSummaryDto>> GetSalesSummary(Guid id)
        {
            var summary = await _userService.GetSalesSummaryAsync(id);
            if (summary == null) return NotFound();
            return Ok(summary);
        }

        public class PinRequest
        {
            public string Pin { get; set; } = string.Empty;
        }

        [HttpPost("set-pin")]
        [Authorize]
        public async Task<IActionResult> SetPin([FromBody] PinRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Pin) || request.Pin.Length != 4 || !request.Pin.All(char.IsDigit))
                return BadRequest("PIN must be exactly 4 digits.");

            var userId = User.GetUserId();
            if (userId == null) return Unauthorized();

            var user = await _userManager.FindByIdAsync(userId.Value.ToString());
            if (user == null) return NotFound();

            user.HashedPin = _userManager.PasswordHasher.HashPassword(user, request.Pin);
            user.PinLastUpdatedAt = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);
            return Ok();
        }

        [HttpPost("verify-pin")]
        [Authorize]
        public async Task<IActionResult> VerifyPin([FromBody] PinRequest request)
        {
            var userId = User.GetUserId();
            if (userId == null) return Unauthorized();

            var user = await _userManager.FindByIdAsync(userId.Value.ToString());
            if (user == null || string.IsNullOrEmpty(user.HashedPin))
                return Unauthorized();

            var result = _userManager.PasswordHasher.VerifyHashedPassword(user, user.HashedPin, request.Pin);
            if (result == PasswordVerificationResult.Success)
            {
                await _activityLogger.LogAsync(user.Id, "PinSuccess", null, HttpContext.Connection.RemoteIpAddress?.ToString());
                return Ok();
            }
            await _activityLogger.LogAsync(user.Id, "PinFailed", new { request.Pin }, HttpContext.Connection.RemoteIpAddress?.ToString());
            return Unauthorized();
        }

        [HttpPost("{id:guid}/reset-pin")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ResetPin(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null) return NotFound();
            user.HashedPin = null;
            user.PinLastUpdatedAt = null;
            await _userManager.UpdateAsync(user);
            return Ok();
        }

        // Bu endpoint kullanıcı oturumunu sonlandırır.
        // Erişim: Giriş yapmış tüm kullanıcılar.
        // Girdi: Yok
        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var userId = User.GetUserId();
            if (userId == null) return Unauthorized();
            await _workSessionService.EndSessionAsync(userId.Value);
            // Kullanıcıya çevrimdışı olduğunu bildir
            await _notificationService.SendToUserAsync(userId.Value, "UserStatusChanged", new { isOnline = false, lastSeen = DateTime.UtcNow });
            return Ok();
        }

        // Bu endpoint admin tarafından kullanıcıyı siler.
        // Erişim: Admin
        // Girdi: Yok
        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var result = await _mediator.Send(new DeleteUserCommand { UserId = id });
            if (result.Succeeded) return Ok();
            return BadRequest(result.Errors);
        }
    }
}
