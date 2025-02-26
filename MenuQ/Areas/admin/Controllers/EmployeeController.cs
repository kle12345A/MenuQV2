using BussinessObject.account;
using BussinessObject.employee;
using DataAccess.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using X.PagedList;

namespace MenuQ.Areas.admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class EmployeeController : Controller
    {
        private readonly IEmployeeService _employeeService;
        private readonly IAccountService _accountService;

        public EmployeeController(IEmployeeService employeeService, IAccountService accountService)
        {
            _employeeService = employeeService;
            _accountService = accountService;
        }

        public async Task<IActionResult> Index(int? page)
        {
            var employees = await _employeeService.GetAllEmployee();

            // Cấu hình phân trang
            int pageNumber = page ?? 1; // Trang hiện tại, mặc định là 1 nếu không có tham số
            int pageSize = 3; // Số mục trên mỗi trang, bạn có thể thay đổi

            // Chuyển danh sách nhân viên thành PagedList
            var pagedEmployees = employees.ToPagedList(pageNumber, pageSize);

            return View(pagedEmployees);
        }

        // Hiển thị form tạo nhân viên
        public async Task<IActionResult> Create()
        {
            var accounts = await _accountService.GetAllAccount();
            ViewBag.AccountList = new SelectList(
                accounts.Select(a => new SelectListItem
                {
                    Value = a.AccountId.ToString(),
                    Text = a.UserName
                }),
                "Value",
                "Text"
            );
            return View(new Employee());
        }

        // Xử lý tạo nhân viên
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Employee employee)
        {
            if (employee.AccountId == null)
            {
                ModelState.AddModelError("AccountId", "Vui lòng chọn tài khoản!");
                var accounts = await _accountService.GetAllAccount();
                ViewBag.AccountList = new SelectList(
                    accounts.Select(a => new SelectListItem
                    {
                        Value = a.AccountId.ToString(),
                        Text = a.UserName,
                    }),
                    "Value",
                    "Text"
                );
                return View(employee);
            }

            await _employeeService.AddAsync(employee);
            TempData["SuccessMessage"] = "Tạo nhân viên thành công!";
            return RedirectToAction("Index");
        }

        // Hiển thị form chỉnh sửa nhân viên
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var employee = await _employeeService.GetByIdAsync(id);
            if (employee == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy nhân viên!";
                return RedirectToAction("Index");
            }

            var accounts = await _accountService.GetAllAccount();
            ViewBag.AccountList = new SelectList(
                accounts.Select(a => new SelectListItem
                {
                    Value = a.AccountId.ToString(),
                    Text = a.UserName
                }),
                "Value",
                "Text",
                employee.AccountId
            );

            return View(employee);
        }

        // Xử lý cập nhật nhân viên
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Employee employee)
        {
            if (employee.AccountId == null)
            {
                ModelState.AddModelError("AccountId", "Vui lòng chọn tài khoản!");
                var accounts = await _accountService.GetAllAccount();
                ViewBag.AccountList = new SelectList(
                    accounts.Select(a => new SelectListItem
                    {
                        Value = a.AccountId.ToString(),
                        Text = a.UserName,
                    }),
                    "Value",
                    "Text",
                    employee.AccountId
                );
                return View(employee);
            }

            var result = await _employeeService.UpdateAsync(employee);
            if (result == 0)
            {
                TempData["ErrorMessage"] = "Không tìm thấy nhân viên!";
                return RedirectToAction("Index");
            }

            TempData["SuccessMessage"] = "Cập nhật nhân viên thành công!";
            return RedirectToAction("Index");
        }
    }
}
