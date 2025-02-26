using BussinessObject.customer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using X.PagedList;

namespace MenuQ.Areas.admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class CustomerController : Controller
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        public async Task<IActionResult> Index(int? page)
        {
            var customers = await _customerService.GetAllAsync();

            
            int pageNumber = page ?? 1; 
            int pageSize = 5;

           
            var pagedCustomers = customers.ToPagedList(pageNumber, pageSize);

            return View(pagedCustomers);
        }

        public async Task<IActionResult> OrderHistory(int customerId)
        {
            var orderHistory = await _customerService.GetOrderHistoryByCustomerId(customerId);
            return View(orderHistory);
        }
    }
}
