using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using eStoreAPI.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BusinessObject.Models;
using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace eStoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<AspNetUsers> _userManager;
        private readonly RoleManager<AspNetRoles> _roleManager; // Thêm RoleManager
        private readonly SignInManager<AspNetUsers> _signInManager;
        private readonly IConfiguration _configuration;

        public AccountController(UserManager<AspNetUsers> userManager, RoleManager<AspNetRoles> roleManager, SignInManager<AspNetUsers> signInManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _roleManager = roleManager;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            var user = new AspNetUsers
            {
                Id = Guid.NewGuid().ToString(),
                UserName = model.Email,
                Email = model.Email,
                EmailConfirmed = true,
                FirstName = model.FirstName,
                LastName = model.LastName
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                // Kiểm tra và tạo vai trò "User" nếu chưa tồn tại
                if (!await _roleManager.RoleExistsAsync("User"))
                {
                    var role = new AspNetRoles
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "User",
                        NormalizedName = "USER"
                    };
                    var roleResult = await _roleManager.CreateAsync(role);
                    if (!roleResult.Succeeded)
                    {
                        return BadRequest(new { Errors = roleResult.Errors.Select(e => e.Description) });
                    }
                }

                // Gán vai trò "User" cho người dùng
                var addToRoleResult = await _userManager.AddToRoleAsync(user, "User");
                if (!addToRoleResult.Succeeded)
                {
                    return BadRequest(new { Errors = addToRoleResult.Errors.Select(e => e.Description) });
                }

                return Ok(new { Message = "User registered successfully with role 'User'" });
            }
            return BadRequest(new { Errors = result.Errors.Select(e => e.Description) });
        }

 
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return BadRequest(new { Message = "Invalid email or password." });
            }

            // Kiểm tra trạng thái khóa tài khoản
            if (await _userManager.IsLockedOutAsync(user))
            {
                var lockoutEnd = await _userManager.GetLockoutEndDateAsync(user);
                return BadRequest(new { Message = $"Tài khoản bị khóa cho đến khi {lockoutEnd?.LocalDateTime}. Vui lòng thử lại sau." });
            }

            var result = await _signInManager.PasswordSignInAsync(user.UserName, model.Password, false, lockoutOnFailure: true);
            if (result.Succeeded)
            {
                // Reset số lần thất bại khi đăng nhập thành công
                await _userManager.ResetAccessFailedCountAsync(user);

                // Lấy roles của user
                var roles = await _userManager.GetRolesAsync(user);

                // Tạo JWT token (gọi phương thức với tham số roles)
                var token = GenerateJwtToken(user, roles);
                return Ok(new { Token = token });
            }
            else
            {
                // Tăng số lần thất bại và khóa nếu cần
                await _userManager.AccessFailedAsync(user);
                var failedAttempts = await _userManager.GetAccessFailedCountAsync(user);
                var maxAttempts = _userManager.Options.Lockout.MaxFailedAccessAttempts;

                if (failedAttempts >= maxAttempts)
                {
                    return BadRequest(new { Message = $"Tài khoản đã bị khóa do {maxAttempts} lần đăng nhập không thành công. Hãy thử lại sau 5 phút." });
                }

                return BadRequest(new { Message = $"đăng nhập không hợp lệ. {maxAttempts - failedAttempts} lần thử còn lại." });
            }
        }



        [Authorize]
        [HttpGet("getUserId")]
        public async Task<IActionResult> GetUserId()
        {
            Console.WriteLine($"User claims: {string.Join(", ", User.Claims.Select(c => $"{c.Type}: {c.Value}"))}");
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                Console.WriteLine("User not found in GetUserId");
                return NotFound("User not found");
            }
            Console.WriteLine($"User ID retrieved: {user.Id}");
            return Ok(user.Id);
        }


        private string GenerateJwtToken(AspNetUsers user, IList<string> roles)
        {
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id),
        new Claim(ClaimTypes.Email, user.Email)
    };

            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


    }
}

