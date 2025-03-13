using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace eStore.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<RegisterModel> _logger;

        public RegisterModel(IHttpClientFactory clientFactory, ILogger<RegisterModel> logger)
        {
            _clientFactory = clientFactory;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            public string Email { get; set; }
            public string Password { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
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

            var client = _clientFactory.CreateClient();

            // Tăng thời gian timeout lên 60 giây (hoặc tùy chỉnh giá trị này theo yêu cầu)
            client.Timeout = TimeSpan.FromSeconds(60);

            var content = new StringContent(JsonSerializer.Serialize(Input), Encoding.UTF8, "application/json");

            try
            {
                var response = await client.PostAsync("https://localhost:7029/api/Account/register", content);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToPage("/Account/Login");
                }

                // Log phản hồi khi không thành công
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("Registration failed. Status code: {StatusCode}, Response: {ErrorContent}", response.StatusCode, errorContent);

                ModelState.AddModelError(string.Empty, "Registration failed.");
                return Page();
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError("Request failed: {Message}", ex.Message);
                _logger.LogError("Stack trace: {StackTrace}", ex.StackTrace); // Log StackTrace để dễ debug

                ModelState.AddModelError(string.Empty, "An error occurred while sending the request. Please try again later.");
                return Page();
            }

        }

    }
}