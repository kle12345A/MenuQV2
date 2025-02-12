using BussinessObject.menu;
using MenuQ.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace MenuQ.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private IMenuItemService _menuItemService;

        public HomeController(ILogger<HomeController> logger, IMenuItemService menuItemService)
        {
            _menuItemService = menuItemService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var menu = await _menuItemService.GetAllAsync();

            return View(menu);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
