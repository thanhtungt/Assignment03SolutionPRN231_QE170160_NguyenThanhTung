using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace eStore.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class EditProductModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<EditProductModel> _logger;

        [BindProperty]
        public ProductDTO Product { get; set; }

        public List<CategoryDTO> Categories { get; set; } // Danh sách các danh mục

        public EditProductModel(IHttpClientFactory clientFactory, ILogger<EditProductModel> logger)
        {
            _clientFactory = clientFactory;
            _logger = logger;
        }

        // Lấy danh sách danh mục và sản phẩm theo ID
        public async Task OnGetAsync(int id)
        {
            var client = _clientFactory.CreateClient("ApiClient");
            client.Timeout = TimeSpan.FromSeconds(60);

            // Lấy thông tin sản phẩm theo ID
            var productResponse = await client.GetAsync($"https://localhost:7029/api/Products/{id}");
            if (productResponse.IsSuccessStatusCode)
            {
                Product = await productResponse.Content.ReadFromJsonAsync<ProductDTO>();
                _logger.LogInformation($"Fetched product with ID: {Product.ProductId}");
            }
            else
            {
                _logger.LogError($"Failed to fetch product. Status code: {productResponse.StatusCode}");
                TempData["Error"] = "Product not found or could not be fetched.";
            }

            // Lấy danh sách danh mục
            var categoryResponse = await client.GetAsync("https://localhost:7029/api/Categories");
            if (categoryResponse.IsSuccessStatusCode)
            {
                var json = await categoryResponse.Content.ReadAsStringAsync();
                Categories = JsonConvert.DeserializeObject<List<CategoryDTO>>(json);
            }
            else
            {
                _logger.LogError($"Failed to fetch categories. Status code: {categoryResponse.StatusCode}");
                Categories = new List<CategoryDTO>();
            }
        }

        // Phương thức xử lý khi người dùng submit form
        public async Task<IActionResult> OnPostAsync()
        {
            if (Product == null || Product.ProductId == 0)
            {
                _logger.LogError("Invalid product ID.");
                return Page();
            }

            var client = _clientFactory.CreateClient("ApiClient");

            // Gửi yêu cầu PUT để cập nhật sản phẩm
            var response = await client.PutAsJsonAsync($"https://localhost:7029/api/Products/{Product.ProductId}", Product);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToPage("ProductManagementModel");
            }
            else
            {
                _logger.LogError($"Failed to update product. Status code: {response.StatusCode}");
                return Page(); // Lấy lại trang nếu thất bại
            }
        }
    }
}
