using BusinessObject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace eStore.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class UsersListModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<UsersListModel> _logger;

        public List<AspNetUsers> Users { get; set; }

        public UsersListModel(IHttpClientFactory clientFactory, ILogger<UsersListModel> logger)
        {
            _clientFactory = clientFactory;
            _logger = logger;
        }

        public async Task OnGetAsync()
        {
            var client = _clientFactory.CreateClient("ApiClient");
            var token = HttpContext.Session.GetString("JWToken");
            if (string.IsNullOrEmpty(token))
            {
                _logger.LogError("JWT Token not found in session");
                Users = new List<AspNetUsers>();
                TempData["Error"] = "Please log in as Admin.";
                return;
            }
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await client.GetAsync("https://localhost:7029/api/Users/alluser");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                Users = JsonConvert.DeserializeObject<List<AspNetUsers>>(json);
                _logger.LogInformation($"Fetched users: {json}");
            }
            else
            {
                _logger.LogError($"Failed to fetch users. Status code: {response.StatusCode}");
                Users = new List<AspNetUsers>();
                TempData["Error"] = "Failed to load user list.";
            }
        }
    }
}