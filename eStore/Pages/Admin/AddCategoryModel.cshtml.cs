using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace eStore.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class AddCategoryModel : PageModel
    {
        private readonly ILogger<AddCategoryModel> _logger;
        private readonly IHttpClientFactory _clientFactory;

        [BindProperty]
        public CategoryDTO Category { get; set; }

        public AddCategoryModel(ILogger<AddCategoryModel> logger, IHttpClientFactory clientFactory)
        {
            _logger = logger;
            _clientFactory = clientFactory;
        }


        public void OnGet()
        {
            Category = new CategoryDTO();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var client = _clientFactory.CreateClient("ApiClient");

            var response = await client.PostAsJsonAsync("https://localhost:7029/api/Categories", Category);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToPage("CategoryListModel");
            }
            else
            {
                _logger.LogError($"Thêm danh mục không thành công. Mã trạng thái: {response.StatusCode}");
                return Page();
            }
        }



    }
}
