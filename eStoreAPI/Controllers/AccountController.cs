using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using eStoreAPI.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BusinessObject.Models;
using Microsoft.AspNetCore.Authorization;

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
            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, false);
            if (result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                var roles = await _userManager.GetRolesAsync(user);
                var token = GenerateJwtToken(user, roles);
                return Ok(new LoginResponseDto { Token = token, Roles = roles });
            }
            return Unauthorized(new { Message = "Invalid login attempt" });
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
        new Claim(JwtRegisteredClaimNames.Sub, user.Email),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim(ClaimTypes.NameIdentifier, user.Id)
    };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var jwtKey = _configuration["JwtSettings:Key"]; // Sửa thành "JwtSettings:Key"
            if (string.IsNullOrEmpty(jwtKey))
            {
                throw new ArgumentNullException("JwtSettings:Key", "JWT Key is missing in configuration.");
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(1);

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"], // Sửa thành "JwtSettings:Issuer"
                audience: _configuration["JwtSettings:Audience"], // Sửa thành "JwtSettings:Audience"
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}