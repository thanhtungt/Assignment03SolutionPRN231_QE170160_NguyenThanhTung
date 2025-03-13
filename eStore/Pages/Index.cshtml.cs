using BusinessObject.Models;
using DataAccess.Contexts;
using eStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace eStore.Pages
{
    /*public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IHttpClientFactory _clientFactory;
        

        public IndexModel(ILogger<IndexModel> logger, IHttpClientFactory clientFactory)
        {
            _logger = logger;
            _clientFactory = clientFactory;
           
        }

        public List<ProductDTO>? Products { get; private set; }
       

        public async Task OnGetAsync()
        {
            // Kiểm tra nếu người dùng là người dùng bình thường (user)
            if (User.Identity.IsAuthenticated && !User.IsInRole("Admin"))
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
                    Products = new List<ProductDTO>();  // Hoặc bạn có thể hiển thị thông báo lỗi
                }
            }
            else
            {
                // Nếu người dùng là Admin, chuyển hướng đến trang Admin Dashboard
                Response.Redirect("/Admin/AdminDashboard");
            }
           
        }

    }*/
    // IndexModel.cshtml.cs
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IHttpClientFactory _clientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ApplicationDbContext _context;

        public IndexModel(ILogger<IndexModel> logger, IHttpClientFactory clientFactory, IHttpContextAccessor httpContextAccessor, ApplicationDbContext context)
        {
            _logger = logger;
            _clientFactory = clientFactory;
            _httpContextAccessor = httpContextAccessor;
            _context = context;
        }

        public List<ProductDTO>? Products { get; private set; }

        public async Task OnGetAsync()
        {
            if (User.Identity.IsAuthenticated && !User.IsInRole("Admin"))
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
                    Products = new List<ProductDTO>();
                }
            }
            else
            {
                Response.Redirect("/Admin/AdminDashboard");
            }
        }

        // Thêm sản phẩm vào giỏ hàng
        public IActionResult OnPostAddToCart(int productId, string productName, decimal unitPrice, int categoryId)
        {
            var cart = GetCartFromSession();
            // Tra cứu CategoryName từ CategoryId
            var categoryName = _context.Categories
                .Where(c => c.CategoryId == categoryId)
                .Select(c => c.CategoryName)
                .FirstOrDefault() ?? "Unknown";
            cart.AddToCart(productId, productName, unitPrice, categoryName);
            SaveCartToSession(cart);
            _logger.LogInformation($"Cart after adding: {JsonConvert.SerializeObject(cart)}");
            return RedirectToPage();
        }




        // Lưu giỏ hàng vào session
        private void SaveCartToSession(ShoppingCart cart)
        {
            var cartJson = JsonConvert.SerializeObject(cart);
            _logger.LogInformation($"Saving cart to session: {cartJson}");
            _httpContextAccessor.HttpContext.Session.SetString("Cart", cartJson);
        }

        // Lấy giỏ hàng từ session
        private ShoppingCart GetCartFromSession()
        {
            var sessionCart = _httpContextAccessor.HttpContext.Session.GetString("Cart");

            if (string.IsNullOrEmpty(sessionCart))
            {
                // Nếu session trống, trả về giỏ hàng mới
                return new ShoppingCart();
            }

            // Giải mã giỏ hàng từ session
            return JsonConvert.DeserializeObject<ShoppingCart>(sessionCart);
        }

    }


}
