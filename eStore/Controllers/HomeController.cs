using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eStore.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        public IActionResult AdminOnly()
        {
            return Content("This is an admin-only page.");
        }
    }
}