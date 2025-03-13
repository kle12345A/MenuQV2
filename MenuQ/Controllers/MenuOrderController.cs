using BussinessObject.customer;
using BussinessObject.DTOs;
using BussinessObject.menu;
using BussinessObject.request;
using DataAccess.Models;
using MenuQ.Hubs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace MenuQ.Controllers
{
    public class MenuOrderController : Controller
    {

        private readonly ILogger<MenuOrderController> _logger;
        private readonly IHubContext<ServerHub> _hub;
        private readonly IMenuItemService _menuService;
        private readonly IRequestService _requestService;
        private readonly ICustomerService _customerService;
        public MenuOrderController(IMenuItemService menuService, IRequestService requestService,ICustomerService customerService,
            IHubContext<ServerHub> hub,ILogger<MenuOrderController> logger)
        {
            _menuService = menuService;
            _requestService = requestService;
            _customerService = customerService;
            _hub = hub;
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
            string username = Request.Cookies["username"];
            int tableId = int.Parse(Request.Cookies["tableId"]);
            Customer customer = await _customerService.GetCustomerByPhone(username);
            OrderByDto detail = new OrderByDto
            {
                TableId = tableId,
                CustomerId = customer.CustomerId,
            };

            await _requestService.AddRequestOrder(cartItems, detail);
            _hub.Clients.All.SendAsync("LoadRequest");
            return Ok(new { message = "Đặt món thành công!" });
        }

        // Add danh sách order

    }

    
}
