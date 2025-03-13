using BusinessObject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

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

        public async Task OnGet()
        {
            var client = _clientFactory.CreateClient("ApiClient");
            var response = await client.GetAsync("https://localhost:7029/api/Users/alluser");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                Users = JsonConvert.DeserializeObject<List<AspNetUsers>>(json);
            }
            else
            {
                _logger.LogError($"Failed to fetch users. Status code: {response.StatusCode}");
            }
        }
    }
}
