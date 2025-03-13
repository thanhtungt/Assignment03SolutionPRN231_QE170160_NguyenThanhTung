using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Net.Http;
using System.Security.Claims;

namespace eStore.Pages.Users
{
    public class OrderHistoryModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor; // Thêm để lấy token từ Session

        public OrderHistoryModel(IHttpClientFactory clientFactory, IHttpContextAccessor httpContextAccessor)
        {
            _clientFactory = clientFactory;
            _httpContextAccessor = httpContextAccessor;
        }

        public List<OrderDTO> Orders { get; private set; }

        public async Task OnGetAsync()
        {
            if (User.Identity.IsAuthenticated)
            {
                var client = _clientFactory.CreateClient("ApiClient");
                var token = _httpContextAccessor.HttpContext.Session.GetString("JWToken");
                if (string.IsNullOrEmpty(token))
                {
                    Console.WriteLine("JWT Token not found in session");
                    Orders = new List<OrderDTO>();
                    return;
                }

                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                // Lấy MemberId từ API
                var userIdResponse = await client.GetAsync("https://localhost:7029/api/Account/getUserId");
                string memberId;
                if (userIdResponse.IsSuccessStatusCode)
                {
                    memberId = await userIdResponse.Content.ReadAsStringAsync();
                    Console.WriteLine($"MemberId from API: {memberId}");
                }
                else
                {
                    Console.WriteLine($"Failed to get MemberId: {await userIdResponse.Content.ReadAsStringAsync()}");
                    Orders = new List<OrderDTO>();
                    return;
                }

                // Lấy danh sách đơn hàng
                var response = await client.GetAsync("api/Orders");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Orders retrieved: {json}");
                    var allOrders = JsonConvert.DeserializeObject<List<OrderDTO>>(json);
                    Orders = allOrders.Where(o => o.MemberId == memberId)
                                     .OrderByDescending(o => o.OrderDate)
                                     .ToList();

                    foreach (var order in Orders)
                    {
                        var detailsResponse = await client.GetAsync($"api/OrderDetails/{order.OrderId}");
                        if (detailsResponse.IsSuccessStatusCode)
                        {
                            var detailsJson = await detailsResponse.Content.ReadAsStringAsync();
                            order.OrderDetails = JsonConvert.DeserializeObject<List<OrderDetailDTO>>(detailsJson);
                        }
                    }
                }
                else
                {
                    Orders = new List<OrderDTO>();
                }
            }
            else
            {
                Orders = new List<OrderDTO>();
            }
        }
    }
}