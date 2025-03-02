using BussinessObject.customer;
using BussinessObject.DTOs;
using BussinessObject.request;
using BussinessObject.table;
using DataAccess.Enum;
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
                Response.Cookies.Append("username", "123456789");
                Response.Cookies.Append("table", "1");
            }

            username = Request.Cookies["username"];

            Customer customer = await _customerService.GetCustomerByPhone("123456789");

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
            try
            {
                int customerId = int.Parse(form["CustomerId"]);
                int tableId = int.Parse(form["TableId"]);
                PaymentMethod paymentMethod = (PaymentMethod)int.Parse(form["paymentMethod"]);

                var paymentRequestDto = new PaymentRequestDTO
                {
                    CustomerId = customerId,
                    TableId = tableId,
                    PaymentMethod = paymentMethod
                };

                var result = await _requestService.CreatePaymentRequest(paymentRequestDto);
                if (!result.Success)
                {
                    TempData["ErrorMessage"] = result.Message;
                    return RedirectToAction("PayOrder");
                }

                TempData["SuccessMessage"] = "Payment request created successfully.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating payment request.");
                TempData["ErrorMessage"] = "An error occurred while processing payment.";
                return RedirectToAction("PayOrder");
            }
        }
    }
}
