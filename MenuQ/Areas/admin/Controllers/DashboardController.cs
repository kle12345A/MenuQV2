using Microsoft.AspNetCore.Mvc;

namespace MenuQ.Areas.admin.Controllers
{
    [Area("Admin")]
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
