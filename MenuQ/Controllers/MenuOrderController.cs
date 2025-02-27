using BussinessObject.Dto;
using BussinessObject.menu;
using BussinessObject.request;
using DataAccess.Models;
using Microsoft.AspNetCore.Mvc;

namespace MenuQ.Controllers
{
    public class MenuOrderController : Controller
    {

        private readonly ILogger<MenuOrderController> _logger;
        private readonly IMenuItemService _menuService;
        private readonly IRequestService _requestService;
        public MenuOrderController(IMenuItemService menuService, IRequestService requestService,
            ILogger<MenuOrderController> logger)
        {
            _menuService = menuService;
            _requestService = requestService;
            _logger = logger;
        }
        // Hiển thị danh sách menu
        public async Task<IActionResult> Index()
        {
            var menuItems = await _menuService.GetAllAsync();
            _logger.LogInformation($"Converted to list, final count: {menuItems.ToList()}");
            return View(menuItems);

        }

        [HttpPost]
        public async Task<IActionResult> PlaceOrder([FromBody] List<OrderItemDto> cartItems)
        {
            if (cartItems == null || cartItems.Count == 0)
            {
                return BadRequest("Giỏ hàng trống hoặc dữ liệu không đúng định dạng.");
            }

            OrderByDto detail = new OrderByDto
            {
                TableId = 1,
                CustomerId = 1,
            };

            await _requestService.AddRequestOrder(cartItems, detail);

            return Ok(new { message = "Đặt món thành công!" });
        }

        // Add danh sách order

    }

    
}
