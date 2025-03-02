using BussinessObject.customer;
using BussinessObject.DTOs;
using BussinessObject.request;
using BussinessObject.table;
using DataAccess.Enum;
using DataAccess.Models;
using DataAccess.Repository.menuitem;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
            int tableId = int.Parse(Request.Cookies["tableId"]);

            Customer customer = await _customerService.GetCustomerByPhone(username);

            Table cTable = await _tableService.GetTableByIdAsync(tableId);
            ViewBag.Table = cTable;

            return View(customer);
        }

        [HttpGet]
        public async Task<IActionResult> PayOrder()
        {
            string username = Request.Cookies["username"];
            int tableId = int.Parse(Request.Cookies["tableId"]);
            Customer customer = await _customerService.GetCustomerByPhone(username);
            var customerId = customer.CustomerId;
            Request OrderDetail = await _requestService.GetPendingFoodOrderRequest(customerId);
            return View(OrderDetail);
        }

        [HttpPost]
        public async Task<IActionResult> PaymentRequest(IFormCollection form)
        {
            try
            {
                string username = Request.Cookies["username"];
                int tableId = int.Parse(Request.Cookies["tableId"]);
                Customer customer = await _customerService.GetCustomerByPhone(username);
                var customerId = customer.CustomerId;

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

        [HttpGet]
        public async Task<IActionResult> Login([FromQuery] int tableId)
        {
            if(tableId == 0)
            {
                return View("/Home/AccessDenied");
            }
            else
            {
                Response.Cookies.Append("tableId", tableId.ToString(), new CookieOptions
                {
                    Expires = DateTime.UtcNow.AddHours(3),
                    HttpOnly = true,
                    Secure = true,
                });
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginCustomerDto dto)
        {
            var customer = await _customerService.CustomerLogin(dto.PhoneNumber, dto.Username);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, customer.PhoneNumber),
                new Claim(ClaimTypes.NameIdentifier, customer.PhoneNumber),
                new Claim(ClaimTypes.Role, "Customer"),
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = false,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(2)
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
            Response.Cookies.Append("username", customer.PhoneNumber, new CookieOptions
            {
                Expires = DateTime.UtcNow.AddHours(3),
                HttpOnly = true,
                Secure = true,
            });
            return RedirectToAction("Index");
        }
    }
}
