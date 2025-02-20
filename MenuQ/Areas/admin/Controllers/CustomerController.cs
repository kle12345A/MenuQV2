using BussinessObject.customer;
using Microsoft.AspNetCore.Mvc;

namespace MenuQ.Areas.admin.Controllers
{
    [Area("Admin")]
    public class CustomerController : Controller
    {
        private readonly ICustomerService _customerService;
        public CustomerController(ICustomerService customerService) {
            _customerService = customerService;
        }
        public async Task<IActionResult> Index()
        {
            var cus = await _customerService.GetAllAsync();
            return View(cus);
        }
        public async Task<IActionResult> OrderHistory(int customerId)
        {
            var orderHistory = await _customerService.GetOrderHistoryByCustomerId(customerId);
            return View(orderHistory);
        }

    }
}
