using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace eStore.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class SalesReportModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<SalesReportModel> _logger;

        public SalesReportModel(IHttpClientFactory clientFactory, ILogger<SalesReportModel> logger)
        {
            _clientFactory = clientFactory;
            _logger = logger;
        }

        [BindProperty]
        public DateTime StartDate { get; set; } = DateTime.Now.AddMonths(-1); // Mặc định 1 tháng trước
        [BindProperty]
        public DateTime EndDate { get; set; } = DateTime.Now; // Mặc định hôm nay

        public List<SalesReportItem> SalesReport { get; set; }

        public class SalesReportItem
        {
            public int OrderId { get; set; }
            public string MemberId { get; set; }
            public DateTime OrderDate { get; set; }
            public decimal Freight { get; set; }
            public decimal TotalSales { get; set; }
        }

        public async Task OnGetAsync()
        {
            await FetchSalesReport();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await FetchSalesReport();
            return Page();
        }

        private async Task FetchSalesReport()
        {
            var client = _clientFactory.CreateClient("ApiClient");
            var token = HttpContext.Session.GetString("JWToken");
            if (string.IsNullOrEmpty(token))
            {
                _logger.LogError("JWT Token not found in session");
                TempData["Error"] = "Please log in as Admin.";
                SalesReport = new List<SalesReportItem>();
                return;
            }
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var url = $"https://localhost:7029/api/Orders/sales-report?startDate={StartDate:yyyy-MM-dd}&endDate={EndDate:yyyy-MM-dd}";
            var response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                SalesReport = JsonConvert.DeserializeObject<List<SalesReportItem>>(json);
                _logger.LogInformation($"Fetched sales report: {json}");
            }
            else
            {
                _logger.LogError($"Failed to fetch sales report. Status code: {response.StatusCode}");
                TempData["Error"] = "No sales data found for the specified period.";
                SalesReport = new List<SalesReportItem>();
            }
        }
    }
}