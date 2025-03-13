using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace eStore.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<LoginModel> _logger;
        

        public LoginModel(IHttpClientFactory clientFactory, ILogger<LoginModel> logger)
        {
            _clientFactory = clientFactory ?? throw new ArgumentNullException(nameof(clientFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required]
            public string Email { get; set; }

            [Required]
            public string Password { get; set; }
        }

        public void OnGet()
        {
        }

        
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var client = _clientFactory.CreateClient("ApiClient");
            var content = new StringContent(JsonSerializer.Serialize(Input), Encoding.UTF8, "application/json");

            try
            {
                var response = await client.PostAsync("https://localhost:7029/api/Account/login", content);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<LoginResponse>(json, new JsonSerializerOptions

                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (result?.Token != null)
                    {
                        // Gọi API để lấy UserId
                        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", result.Token);
                        var userIdResponse = await client.GetAsync("https://localhost:7029/api/Account/getUserId");
                        var memberId = await userIdResponse.Content.ReadAsStringAsync();

                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, Input.Email),
                            new Claim(ClaimTypes.NameIdentifier, memberId)
                        };

                        // Lấy roles từ token hoặc API nếu cần
                        var tokenHandler = new JwtSecurityTokenHandler();
                        var jwtToken = tokenHandler.ReadJwtToken(result.Token);
                        var roles = jwtToken.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList();

                        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

                        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        var principal = new ClaimsPrincipal(identity);

                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                        HttpContext.Session.SetString("JWToken", result.Token);

                        if (roles.Contains("Admin"))
                        {
                            return RedirectToPage("/Admin/AdminDashboard");
                        }
                        else
                        {
                            return RedirectToPage("/Index");
                        }
                    }
                    else
                    {
                        _logger.LogError("Failed to deserialize login response.");
                        ErrorMessage = "Failed to process the response.";
                        return Page();
                    }
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Login failed with status code {StatusCode}: {Error}", response.StatusCode, errorContent);
                    ErrorMessage = errorContent; // Hiển thị thông báo lỗi từ API
                    return Page();
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError("Request failed: {Message}", ex.Message);
                ErrorMessage = "There was an error while making the request.";
                return Page();
            }
        }
    }


}

    public class LoginResponse
    {
        public string Token { get; set; }
        public IList<string>? Roles { get; set; }
    }
