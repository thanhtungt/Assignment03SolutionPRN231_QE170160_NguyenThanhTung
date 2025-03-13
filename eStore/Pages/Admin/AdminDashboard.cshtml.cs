using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace eStore.Pages
{
    [Authorize(Roles = "Admin")]
    public class AdminDashboardModel : PageModel
    {
        public void OnGet()
        {
            RedirectToPage("/Account/Login");
        }
    }
}