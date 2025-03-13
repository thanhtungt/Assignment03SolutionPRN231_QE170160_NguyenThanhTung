using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace eStore.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class AddProductModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<AddProductModel> _logger;

        [BindProperty]
        public ProductDTO Product { get; set; }

        public List<CategoryDTO> Categories { get; set; } // Danh sách các danh mục

        public AddProductModel(IHttpClientFactory clientFactory, ILogger<AddProductModel> logger)
        {
            _clientFactory = clientFactory;
            _logger = logger;
        }

        // Lấy danh sách danh mục khi người dùng truy cập trang
        public async Task OnGetAsync()
        {
            var client = _clientFactory.CreateClient("ApiClient");

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

            Product = new ProductDTO(); // Khởi tạo một đối tượng ProductDTO mới khi trang được tải
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var client = _clientFactory.CreateClient("ApiClient");

            var response = await client.PostAsJsonAsync("https://localhost:7029/api/Products", Product);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToPage("ProductManagementModel");
            }
            else
            {
                _logger.LogError($"Failed to add product. Status code: {response.StatusCode}");
                return Page(); // Trả lại trang nếu thất bại
            }
        }
    }
}
