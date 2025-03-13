using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace eStore.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class OrderListModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<OrderListModel> _logger;

        public List<OrderDTO> Orders { get; set; }

        public OrderListModel(IHttpClientFactory clientFactory, ILogger<OrderListModel> logger)
        {
            _clientFactory = clientFactory;
            _logger = logger;
        }

        public async Task OnGetAsync()
        {
            var client = _clientFactory.CreateClient("ApiClient");
            client.Timeout = TimeSpan.FromSeconds(60);
            var response = await client.GetAsync("https://localhost:7029/api/Orders");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                Orders = JsonConvert.DeserializeObject<List<OrderDTO>>(json);
            }
            else
            {
                _logger.LogError($"Failed to fetch orders. Status code: {response.StatusCode}");
                Orders = new List<OrderDTO>();
            }
        }
    }
}
