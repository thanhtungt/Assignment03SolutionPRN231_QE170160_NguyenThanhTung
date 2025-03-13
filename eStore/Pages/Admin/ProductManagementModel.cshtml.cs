using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace eStore.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class ProductManagementModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<ProductManagementModel> _logger;

        public List<ProductDTO> Products { get; set; }

        public ProductManagementModel(IHttpClientFactory clientFactory, ILogger<ProductManagementModel> logger)
        {
            _clientFactory = clientFactory;
            _logger = logger;
        }

        public async Task OnGetAsync()
        {
            var client = _clientFactory.CreateClient("ApiClient");
            client.Timeout = TimeSpan.FromSeconds(60);
            var response = await client.GetAsync("https://localhost:7029/api/Products");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                Products = JsonConvert.DeserializeObject<List<ProductDTO>>(json);
            }
            else
            {
                _logger.LogError($"Failed to fetch products. Status code: {response.StatusCode}");
                Products = new List<ProductDTO>();  // Hoặc thông báo lỗi
            }
        }

    }

}
