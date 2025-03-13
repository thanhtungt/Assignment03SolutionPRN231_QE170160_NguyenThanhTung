using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Net.Http;
using System.Security.Claims;
using System.Text;

namespace eStore.Pages.Users
{
    public class EditProfileModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public EditProfileModel(IHttpClientFactory clientFactory, IHttpContextAccessor httpContextAccessor)
        {
            _clientFactory = clientFactory;
            _httpContextAccessor = httpContextAccessor;
        }

        [BindProperty]
        public AspNetUsersDTO Input { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToPage("/Account/Login");
            }

            var client = _clientFactory.CreateClient("ApiClient");
            var token = _httpContextAccessor.HttpContext.Session.GetString("JWToken");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToPage("/Account/Login");
            }

            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var userIdResponse = await client.GetAsync("https://localhost:7029/api/Account/getUserId");
            if (!userIdResponse.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Không thể lấy thông tin người dùng.");
                return Page();
            }

            var memberId = await userIdResponse.Content.ReadAsStringAsync();
            var userResponse = await client.GetAsync($"https://localhost:7029/api/Users/{memberId}");
            if (userResponse.IsSuccessStatusCode)
            {
                var json = await userResponse.Content.ReadAsStringAsync();
                Input = JsonConvert.DeserializeObject<AspNetUsersDTO>(json);
                Console.WriteLine($"User data loaded: {json}");
            }
            else
            {
                ModelState.AddModelError("", "Không thể tải thông tin người dùng.");
                return Page();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var client = _clientFactory.CreateClient("ApiClient");
            var token = _httpContextAccessor.HttpContext.Session.GetString("JWToken");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToPage("/Account/Login");
            }

            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var userIdResponse = await client.GetAsync("https://localhost:7029/api/Account/getUserId");
            if (!userIdResponse.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Không thể xác định người dùng.");
                return Page();
            }

            var memberId = await userIdResponse.Content.ReadAsStringAsync();
            Input.Id = memberId;

            var json = JsonConvert.SerializeObject(Input);
            Console.WriteLine($"Sending update to API: {json}");
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PutAsync($"https://localhost:7029/api/Users/{memberId}", content);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Cập nhật thông tin thành công.";
                return RedirectToPage("/Users/EditProfile");
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Update failed: {errorContent}");
                ModelState.AddModelError("", $"Cập nhật thất bại: {errorContent}");
                return Page();
            }
        }
    }

    public class AspNetUsersDTO
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}