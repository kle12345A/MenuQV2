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
using BussinessObject.servicereason;
using BussinessObject.servicecall;
using Microsoft.AspNetCore.SignalR;
using MenuQ.Hubs;
using BussinessObject.invoice;
using DataAccess.Models.VnPay;
using BussinessObject.vnpay;

namespace MenuQ.Controllers
{
    public class HomeAppController : Controller
    {
        private readonly ILogger<HomeAppController> _logger;
        private readonly IHubContext<ServerHub> _hub;
        private readonly ICustomerService _customerService;
        private readonly ITableService _tableService;
        private readonly IRequestService _requestService;
        private readonly IInvoiceService _invoiceService;
        private readonly IServiceReasonService _reasonService;
        private readonly IServiceCallService _callService;
        public HomeAppController(ICustomerService customerService,
                             ITableService tableService,
                             IRequestService requestService,
                             IInvoiceService invoiceService,
                             IServiceReasonService serviceReasonService,
                             IServiceCallService serviceCallService,
                             IHubContext<ServerHub> hub,
                             ILogger<HomeAppController> logger)
        {
            _customerService = customerService;
            _tableService = tableService;
            _requestService = requestService;
            _invoiceService = invoiceService;
            _reasonService = serviceReasonService;
            _callService = serviceCallService;
            _hub = hub;
            _logger = logger;
        }
        public async Task<IActionResult> Index()
        {
            string username = Request.Cookies["customerUsername"];
            int tableId = int.Parse(Request.Cookies["tableId"]);

            Customer customer = await _customerService.GetCustomerByPhone(username);

            Table cTable = await _tableService.GetTableByIdAsync(tableId);
            ViewBag.Table = cTable;

            return View(customer);
        }

        [HttpGet]
        public async Task<IActionResult> PayOrder()
        {
            string username = Request.Cookies["customerUsername"];
            int tableId = int.Parse(Request.Cookies["tableId"]);
            Customer customer = await _customerService.GetCustomerByPhone(username);
            var customerId = customer.CustomerId;
            Request OrderDetail = await _requestService.GetPendingFoodOrderRequest(customerId);
            return View(OrderDetail);
        }

        [HttpPost]
        public async Task<IActionResult> PaymentRequest(IFormCollection form, [FromServices] IVnPayService _vnPayService, [FromServices] IConfiguration _configuration)
        {
            try
            {
                string phone = Request.Cookies["customerUsername"];
                int tableId = int.Parse(Request.Cookies["tableId"]);
                Customer customer = await _customerService.GetCustomerByPhone(phone);
                var customerId = customer.CustomerId;

                PaymentMethod paymentMethod = (PaymentMethod)int.Parse(form["paymentMethod"]);

                if (string.IsNullOrEmpty(form["TotalAmount"]) || !double.TryParse(form["TotalAmount"], out double amount))
                {
                    _logger.LogError("PaymentRequest failed: 'TotalAmount' is missing or invalid.");
                    TempData["ErrorMessage"] = "Số tiền thanh toán không hợp lệ.";
                    return RedirectToAction("PayOrder");
                }

                // 🔹 **Nếu khách hàng chọn thanh toán qua VNPAY**
                if (paymentMethod == PaymentMethod.CreditCardAtCounter || paymentMethod == PaymentMethod.CreditCardAtTable)
                {
                    // 🔹 **Tạo request checkout với RequestType = 3 và lấy `RequestId`**
                    var checkoutRequestResult = await _requestService.CreateVnPayRequestAsync(customerId, tableId);
                    if (!checkoutRequestResult.Success)
                    {
                        TempData["ErrorMessage"] = checkoutRequestResult.Message;
                        return RedirectToAction("PayOrder");
                    }

                    var checkoutRequest = checkoutRequestResult.Data; // 🆕 Lấy `RequestId`

                    var vnpayConfig = _configuration.GetSection("Vnpay");
                    bool hasVnPay = !string.IsNullOrEmpty(vnpayConfig["TmnCode"]) && !string.IsNullOrEmpty(vnpayConfig["HashSecret"]);

                    if (hasVnPay)
                    {
                        var paymentModel = new PaymentInformationModel
                        {
                            OrderId = checkoutRequest.RequestId, // 🆕 Truyền RequestId từ DB
                            OrderType = "order",
                            Amount = amount,
                            OrderDescription = $"Thanh toán bàn {tableId}",
                            Name = customer.CustomerName
                        };

                        string paymentUrl = _vnPayService.CreatePaymentUrl(paymentModel, HttpContext);
                        if (!string.IsNullOrEmpty(paymentUrl))
                        {
                            return Redirect(paymentUrl); // Chuyển hướng đến cổng VNPAY
                        }
                    }
                }

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

                _hub.Clients.All.SendAsync("LoadRequest");
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing payment request.");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi xử lý thanh toán.";
                return RedirectToAction("PayOrder");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Login([FromQuery] int? tableId)
        {
            var tableInCookie = Request.Cookies["tableId"];
            if (tableId == null)
            {
                if (tableInCookie != null)
                {
                    Response.Cookies.Append("tableId", tableInCookie, new CookieOptions
                    {
                        Expires = DateTime.UtcNow.AddHours(3),
                        HttpOnly = true,
                    });
                    var currentCus = await _customerService.GetCustomerByPhone(Request.Cookies["customerUsername"]);
                    LoginCustomerDto dto = new LoginCustomerDto
                    {
                        Username = currentCus.CustomerName,
                        PhoneNumber = currentCus.PhoneNumber,
                    };
                    return View(dto);
                }
                else
                    return View("/Home/AccessDenied");
            }
            else
            {
                Response.Cookies.Append("tableId", tableId.ToString(), new CookieOptions
                {
                    Expires = DateTime.UtcNow.AddHours(3),
                    HttpOnly = true,
                });
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginCustomerDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }
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
            Response.Cookies.Append("customerUsername", customer.PhoneNumber, new CookieOptions
            {
                Expires = DateTime.UtcNow.AddHours(3),
                HttpOnly = true,
                Secure = false,
                SameSite = SameSiteMode.Lax
            });
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> CallService([FromQuery] int customerId, int tableId)
        {
            ServiceCallResponseDto serviceCallDto = new ServiceCallResponseDto
            {
                customerId = customerId,
                tableId = tableId,
                ListService = await _reasonService.GetAllActive(),
            };
            return PartialView("_CallService", serviceCallDto);
        }

        [HttpPost]
        public async Task<IActionResult> CallService(ServiceCallResponseDto serviceCallDto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError("ModelState Errors: {errors}",
    string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));

                return Json(new { success = false, message = "Invalid data" });
            }
            try
            {
                _logger.LogError("day la lỗi gửi" + serviceCallDto.CustomService);

                await _requestService.AddRequestService(serviceCallDto);
                _hub.Clients.All.SendAsync("LoadRequest");
                return Json(new { success = true, message = "Call Service successfully!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error call service");
                return Json(new { success = false, message = "Failed to call service." });
            }
        }
    }
}
