
using BusinessObject.Models;
using eStore.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Net.Http;
using System.Security.Claims;
using System.Text;

namespace eStore.Pages.Users
{
    public class CartModel : PageModel
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IHttpClientFactory _clientFactory; // Thêm IHttpClientFactory để gọi API
       
        public ShoppingCart Cart { get; private set; }

        public CartModel(IHttpContextAccessor httpContextAccessor, IHttpClientFactory clientFactory)
        {
            _httpContextAccessor = httpContextAccessor;
            _clientFactory = clientFactory;
         
        }

        public void OnGet()
        {
            Cart = GetCartFromSession();
            Console.WriteLine($"Cart contains {Cart.Items.Count} items.");
        }

        public IActionResult OnPostRemoveFromCart(int productId)
        {
            Cart = GetCartFromSession();
            Cart.RemoveFromCart(productId);
            SaveCartToSession(Cart);
            return RedirectToPage();
        }

       
        public async Task<IActionResult> OnPostPlaceOrderAsync()
        {
            Cart = GetCartFromSession();
            Console.WriteLine("OnPostPlaceOrderAsync called");
            if (!Cart.Items.Any())
            {
                Console.WriteLine("Cart is empty");
                return Page();
            }

            var client = _clientFactory.CreateClient("ApiClient");
            var token = _httpContextAccessor.HttpContext.Session.GetString("JWToken"); // Lấy token từ Session
            if (string.IsNullOrEmpty(token))
            {
                Console.WriteLine("JWT Token not found in session {token}");
                ModelState.AddModelError("", "Vui lòng đăng nhập lại.");
                return Page();
            }

            // Thêm token vào header Authorization
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var userIdResponse = await client.GetAsync("https://localhost:7029/api/Account/getUserId");
            string memberId;
            if (userIdResponse.IsSuccessStatusCode)
            {
                memberId = await userIdResponse.Content.ReadAsStringAsync();
                Console.WriteLine($"MemberId from API: {memberId}");
            }
            else
            {
                var error = await userIdResponse.Content.ReadAsStringAsync();
                Console.WriteLine($"Failed to get MemberId from API: {error}");
                ModelState.AddModelError("", "Không thể lấy ID người dùng.");
                return Page();
            }

            var orderDto = new OrderDTO
            {
                MemberId = memberId,
                OrderDate = DateTime.Now,
                Freight = 0m,
                OrderDetails = Cart.Items.Select(item => new OrderDetailDTO
                {
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    UnitPrice = item.UnitPrice,
                    Quantity = (short)item.Quantity
                }).ToList()
            };

            var json = JsonConvert.SerializeObject(orderDto);
            Console.WriteLine($"Sending order to API: {json}");
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("https://localhost:7029/api/Orders", content);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Order created successfully");
                _httpContextAccessor.HttpContext.Session.Remove("Cart");
                return RedirectToPage("/Users/OrderHistoryModel");
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Order creation failed: {errorContent}");
                ModelState.AddModelError("", "Đặt hàng thất bại: " + errorContent);
                return Page();
            }
        }

        private void SaveCartToSession(ShoppingCart cart)
        {
            if (cart != null && cart.Items != null)
            {
                var cartJson = JsonConvert.SerializeObject(cart);
                _httpContextAccessor.HttpContext.Session.SetString("Cart", cartJson);
            }
        }

        private ShoppingCart GetCartFromSession()
        {
            var sessionCart = _httpContextAccessor.HttpContext.Session.GetString("Cart");
            return string.IsNullOrEmpty(sessionCart)
                ? new ShoppingCart()
                : JsonConvert.DeserializeObject<ShoppingCart>(sessionCart);
        }
    }
}