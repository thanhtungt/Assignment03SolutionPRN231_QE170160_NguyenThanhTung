using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace eStore.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class EditOrderModel : PageModel
    {
        private readonly ILogger<EditOrderModel> _logger;
        private readonly IHttpClientFactory _clientFactory;

        public EditOrderModel(ILogger<EditOrderModel> logger, IHttpClientFactory clientFactory)
        {
            _logger = logger;
            _clientFactory = clientFactory;
        }

        [BindProperty]
        public OrderDTO Order { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var client = _clientFactory.CreateClient("ApiClient");
            client.Timeout = TimeSpan.FromSeconds(60);

            var response = await client.GetAsync($"https://localhost:7029/api/Orders/{id}");
            if (response.IsSuccessStatusCode)
            {
                Order = await response.Content.ReadFromJsonAsync<OrderDTO>();
                _logger.LogInformation($"Fetched order with ID: {Order.OrderId}, MemberId: {Order.MemberId}");
            }
            else
            {
                _logger.LogError($"Failed to fetch order. Status code: {response.StatusCode}");
                TempData["Error"] = "Order not found or could not be fetched.";
                return RedirectToPage("OrderListModel");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (Order == null || Order.OrderId == 0)
            {
                _logger.LogError("Invalid order ID.");
                TempData["Error"] = "Invalid order data.";
                return Page();
            }

            // Lấy dữ liệu gốc để giữ MemberId không thay đổi
            var client = _clientFactory.CreateClient("ApiClient");
            var originalResponse = await client.GetAsync($"https://localhost:7029/api/Orders/{Order.OrderId}");
            if (!originalResponse.IsSuccessStatusCode)
            {
                _logger.LogError($"Failed to fetch original order. Status code: {originalResponse.StatusCode}");
                TempData["Error"] = "Could not validate original order data.";
                return Page();
            }

            var originalOrder = await originalResponse.Content.ReadFromJsonAsync<OrderDTO>();
            Order.MemberId = originalOrder.MemberId; // Giữ MemberId từ dữ liệu gốc

            var json = JsonConvert.SerializeObject(Order);
            _logger.LogInformation($"Sending order update: {json}");
            var response = await client.PutAsJsonAsync($"https://localhost:7029/api/Orders/{Order.OrderId}", Order);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Order updated successfully.");
                return RedirectToPage("OrderListModel");
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError($"Failed to update order. Status code: {response.StatusCode}, Error: {errorContent}");
                TempData["Error"] = $"Failed to update order: {errorContent}";
                return Page();
            }
        }
    }
}