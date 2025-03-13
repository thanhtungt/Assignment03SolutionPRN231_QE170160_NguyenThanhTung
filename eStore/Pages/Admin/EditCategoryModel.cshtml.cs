using BusinessObject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace eStore.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class EditCategoryModel : PageModel
    {
        private readonly ILogger<EditCategoryModel> _logger;
        private readonly IHttpClientFactory _clientFactory;

        [BindProperty]
        public CategoryDTO Category { get; set; }

        public EditCategoryModel(ILogger<EditCategoryModel> logger, IHttpClientFactory clientFactory)
        {
            _logger = logger;
            _clientFactory = clientFactory;
        }

        public async Task OnGetAsync(int id)
        {
            var client = _clientFactory.CreateClient("ApiClient");
            client.Timeout = TimeSpan.FromSeconds(60);

            var response = await client.GetAsync($"https://localhost:7029/api/Categories/{id}");
            if (response.IsSuccessStatusCode)
            {
                Category = await response.Content.ReadFromJsonAsync<CategoryDTO>();
                
                _logger.LogInformation($"Đã lấy danh mục với ID: {Category.CategoryId}");
            }
            else
            {
                _logger.LogError($"Lấy danh mục không thành công. Mã trạng thái: {response.StatusCode}");
                TempData["Error"] = "Danh mục không tồn tại hoặc không thể lấy thông tin";
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (Category == null || Category.CategoryId == 0)
            {
                _logger.LogError("ID danh mục không hợp lệ.");
                return Page();
            }

            var client = _clientFactory.CreateClient("ApiClient");

            var response = await client.PutAsJsonAsync($"https://localhost:7029/api/Categories/{Category.CategoryId}", Category);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToPage("CategoryListModel");
            }
            else
            {
                _logger.LogError($"Cập nhật danh mục không thành công. Mã trạng thái: {response.StatusCode}");
                TempData["Error"] = "Cập nhật danh mục không thành công, vui lòng kiểm tra lại.";
                return Page();
            }
        }


    }
}
