using Dekofar.HyperConnect.Application.Auth;
using Dekofar.HyperConnect.Application.Interfaces;
using Dekofar.HyperConnect.Application.Common.Interfaces;
using Dekofar.HyperConnect.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Claims;

namespace Dekofar.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    // Kimlik doğrulama ve kullanıcı yönetimi işlemlerini yöneten controller
    public class AuthController : ControllerBase
    {
        // Kullanıcı yönetimi servisi
        private readonly UserManager<ApplicationUser> _userManager;
        // Oturum açma işlemlerini yöneten servis
        private readonly SignInManager<ApplicationUser> _signInManager;
        // JWT token üretimi için servis
        private readonly ITokenService _tokenService;
        private readonly IActivityLogger _activityLogger;
        private readonly IBadgeService _badgeService;
        private readonly IWorkSessionService _workSessionService;

        // Gerekli bağımlılıkları enjekte eden kurucu metot
        public AuthController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ITokenService tokenService,
            IActivityLogger activityLogger,
            IBadgeService badgeService,
            IWorkSessionService workSessionService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _activityLogger = activityLogger;
            _badgeService = badgeService;
            _workSessionService = workSessionService;
        }

        // Birden fazla kullanıcıyı rol bilgisiyle birlikte oluşturur
        [HttpPost("register-multiple")]
        public async Task<IActionResult> RegisterMultiple(
            [FromBody] List<RegisterRoleRequest> requests,
            [FromServices] RoleManager<IdentityRole<Guid>> roleManager)
        {
            var createdUsers = new List<object>();
            var errors = new List<object>();

            foreach (var request in requests)
            {
                var user = new ApplicationUser
                {
                    UserName = request.Email,
                    Email = request.Email,
                    FullName = request.FullName,
                    MembershipDate = DateTime.UtcNow
                };

                var result = await _userManager.CreateAsync(user, request.Password);
                if (!result.Succeeded)
                {
                    errors.Add(new
                    {
                        request.Email,
                        Errors = result.Errors.Select(e => e.Description).ToList()
                    });
                    continue;
                }

                var roleName = string.IsNullOrWhiteSpace(request.Role) ? "Personel" : request.Role;

                if (!await roleManager.RoleExistsAsync(roleName))
                    await roleManager.CreateAsync(new IdentityRole<Guid>(roleName));

                await _userManager.AddToRoleAsync(user, roleName);

                createdUsers.Add(new
                {
                    user.Id,
                    user.Email,
                    user.FullName,
                    Role = roleName
                });
            }

            return Ok(new
            {
                Message = "İşlem tamamlandı.",
                Created = createdUsers,
                Failed = errors
            });
        }

        // Belirli bir rol ile kullanıcı oluşturur
        [HttpPost("register-with-role")]
        public async Task<IActionResult> RegisterWithRole([FromBody] RegisterRoleRequest request,
            [FromServices] UserManager<ApplicationUser> userManager,
            [FromServices] RoleManager<IdentityRole<Guid>> roleManager)
        {
            var user = new ApplicationUser
            {
                UserName = request.Email,
                Email = request.Email,
                FullName = request.FullName,
                MembershipDate = DateTime.UtcNow
            };

            var result = await userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            // Admin rolünü otomatik ata
            if (!await roleManager.RoleExistsAsync("Admin"))
                await roleManager.CreateAsync(new IdentityRole<Guid>("Admin"));

            await userManager.AddToRoleAsync(user, "Admin");

            return Ok(new { message = "Kullanıcı başarıyla oluşturuldu." });
        }

        // Varsayılan kullanıcı rolü ile kayıt işlemi
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] Dekofar.HyperConnect.Application.Auth.RegisterRequest request,
            [FromServices] RoleManager<IdentityRole<Guid>> roleManager)
        {
            var user = new ApplicationUser
            {
                UserName = request.Email,
                Email = request.Email,
                FullName = request.FullName,
                MembershipDate = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            if (!await roleManager.RoleExistsAsync("User"))
                await roleManager.CreateAsync(new IdentityRole<Guid>("User"));

            await _userManager.AddToRoleAsync(user, "User");

            var roles = await _userManager.GetRolesAsync(user);
            var token = _tokenService.GenerateToken(user.Id.ToString(), user.Email!, roles);

            // Oturum başlat ve kullanıcıyı online yap
            await _workSessionService.StartSessionAsync(user.Id, HttpContext.Connection.RemoteIpAddress?.ToString());

            await _activityLogger.LogAsync(user.Id, "Login", null, HttpContext.Connection.RemoteIpAddress?.ToString());
            await _badgeService.EvaluateAsync(user.Id);

            return Ok(new
            {
                token,
                user = new
                {
                    user.Id,
                    user.FullName,
                    user.Email,
                    Roles = roles
                }
            });
        }

        // Rol bilgisi içeren kayıt isteği modeli
        public class RegisterRoleRequest
        {
            // Kullanıcının tam adı
            public string FullName { get; set; } = "";
            // Kullanıcının e-posta adresi
            public string Email { get; set; } = "";
            // Kullanıcı şifresi
            public string Password { get; set; } = "";
            // Opsiyonel atanacak rol
            public string? Role { get; set; }
        }

        // Kullanıcı giriş işlemi yapar
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
                return Unauthorized("Kullanıcı bulunamadı.");

            var signInResult = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
            if (!signInResult.Succeeded)
                return Unauthorized("Şifre hatalı.");

            var roles = await _userManager.GetRolesAsync(user);
            var token = _tokenService.GenerateToken(user.Id.ToString(), user.Email!, roles);

            // Oturum başlat ve kullanıcıyı online yap
            await _workSessionService.StartSessionAsync(user.Id, HttpContext.Connection.RemoteIpAddress?.ToString());

            await _activityLogger.LogAsync(user.Id, "Login", null, HttpContext.Connection.RemoteIpAddress?.ToString());
            await _badgeService.EvaluateAsync(user.Id);

            return Ok(new
            {
                token,
                user = new
                {
                    user.Id,
                    user.FullName,
                    user.Email,
                    Roles = roles
                }
            });
        }

        // Oturum açmış kullanıcının profil bilgilerini döner
        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            var roles = await _userManager.GetRolesAsync(user);

            return Ok(new
            {
                user.Id,
                user.Email,
                user.FullName,
                Roles = roles
            });
        }
    }
}
