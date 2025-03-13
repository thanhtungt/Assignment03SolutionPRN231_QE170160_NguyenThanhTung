using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace eStore.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class CategoryListModelModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<CategoryListModelModel> _logger;

        public List<CategoryDTO> Categories { get; set; }

        public CategoryListModelModel(IHttpClientFactory clientFactory, ILogger<CategoryListModelModel> logger)
        {
            _clientFactory = clientFactory;
            _logger = logger;
        }

        public async Task OnGetAsync()
        {
            var client = _clientFactory.CreateClient("ApiClient");
            client.Timeout = TimeSpan.FromSeconds(60);
            var response = await client.GetAsync("https://localhost:7029/api/Categories");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                Categories = JsonConvert.DeserializeObject<List<CategoryDTO>>(json);
            }
            else
            {
                _logger.LogError($"Failed to fetch categories. Status code: {response.StatusCode}");
                Categories = new List<CategoryDTO>();
            }
        }
      
    }
}
