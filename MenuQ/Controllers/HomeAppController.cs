using BussinessObject.customer;
using BussinessObject.request;
using BussinessObject.table;
using DataAccess.Models;
using DataAccess.Repository.menuitem;
using Microsoft.AspNetCore.Mvc;

namespace MenuQ.Controllers
{
    public class HomeAppController : Controller
    {
        private readonly ILogger<HomeAppController> _logger;
        private readonly ICustomerService _customerService;
        private readonly ITableService _tableService;
        private readonly IRequestService _requestService;
        public HomeAppController(ICustomerService customerService,
                             ITableService tableService,
                             IRequestService requestService,
                             ILogger<HomeAppController> logger)
        {
            _customerService = customerService;
            _tableService = tableService;
            _requestService = requestService;
            _logger = logger;
        }
        public async Task<IActionResult> Index()
        {
            string username = Request.Cookies["username"];
                int tableId = 1;
            if (string.IsNullOrEmpty(username) || tableId == 0)
            {
                CookieOptions options = new CookieOptions
                {
                    Expires = DateTime.Now.AddHours(1)
                };
                Response.Cookies.Append("username", "0988489099");
                Response.Cookies.Append("table", "1");
            }

            username = Request.Cookies["username"];

            Customer customer = await _customerService.GetCustomerByPhone("0988489099");

            Table cTable = await _tableService.GetTableByIdAsync(tableId);
            ViewBag.Table = cTable;

            return View(customer);
        }

        [HttpGet]
        public async Task<IActionResult> PayOrder()
        {
            var customerId = 1;
            Request OrderDetail = await _requestService.GetPendingFoodOrderRequest(customerId);
            return View(OrderDetail);
        }

        [HttpPost]
        public async Task<IActionResult> PaymentRequest(IFormCollection form)
        {
            var paymentMethod = form["paymentMethod"];
            var requestId = form["RequestId"];
            var totalAmout = form["TotalAmount"];
            var tableId = form["TableId"];
            _logger.LogInformation($@"
                ===== Payment Request Info =====
                Request ID: {requestId}
                Total Amount: {totalAmout}
                Table ID: {tableId}
                ================================");
            return RedirectToAction("Index");
        }
    }
}
