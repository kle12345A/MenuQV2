using BussinessObject.account;
using BussinessObject.role;
using DataAccess.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using X.PagedList;
namespace MenuQ.Areas.admin.Controllers
{
    [Area("Admin")]
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly IRoleService _roleService;
        public AccountController(IAccountService accountService, IRoleService roleService)
        {
            _roleService = roleService;
            _accountService = accountService;
        }
        public async Task<IActionResult> Index(int? page)
        {
            var accounts = await _accountService.GetAllAccount();
            var roles = await _roleService.GetAllAsync();

          
            int pageNumber = page ?? 1; 
            int pageSize = 5;

          
            var pagedAccounts = accounts.ToPagedList(pageNumber, pageSize);

            ViewBag.Roles = roles; 

            return View(pagedAccounts);
        }
        public async Task<IActionResult> Create()
        {
            var roles = await _roleService.GetAllAsync();
            ViewBag.Roles = roles;  
            return View(); 
        }
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create(Account accountModel, Employee? employee, Admin? admin)
        //{
        //    int result = await _accountService.AddWithDetailsAsync(accountModel, employee, admin);

        //    if (result == -1)
        //    {
        //        ModelState.AddModelError("", "Tài khoản đã tồn tại.");
        //        return View(accountModel);
        //    }

        //    return RedirectToAction("Index");
        //}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Account account, Employee employee, DataAccess.Models.Admin admin)
        {
            

            try
            {
                // Lấy thông tin vai trò để kiểm tra
                var role = (await _roleService.GetAllAsync()).FirstOrDefault(r => r.RoleId == account.RoleId);
                if (role == null)
                {
                    ModelState.AddModelError("", "Vai trò không hợp lệ.");
                    ViewBag.Roles = await _roleService.GetAllAsync();
                    return View(account);
                }

                // Tạo tài khoản mới
                var newAccount = new Account
                {
                    Email = account.Email,
                    UserName = account.UserName,
                    PhoneNumber = account.PhoneNumber,
                    Password = account.Password, // Mật khẩu sẽ được mã hóa trong AddWithDetailsAsync
                    RoleId = account.RoleId,
                    Active = account.Active
                };

                Employee newEmployee = null;
                DataAccess.Models.Admin newAdmin = null;

                // Xử lý thông tin bổ sung dựa trên vai trò
                var roleName = role.RoleName.ToLower();
                if (roleName.Contains("employee"))
                {
                    if (!string.IsNullOrEmpty(employee.FullName) || !string.IsNullOrEmpty(employee.Position))
                    {
                        newEmployee = new Employee
                        {
                            FullName = employee.FullName,
                            Position = employee.Position,
                            HireDate = employee.HireDate
                        };
                    }
                }
                else if (roleName.Contains("admin"))
                {
                    if (!string.IsNullOrEmpty(admin.FullName) || !string.IsNullOrEmpty(admin.Position))
                    {
                        newAdmin = new DataAccess.Models.Admin
                        {
                            FullName = admin.FullName,
                            Position = admin.Position
                        };
                    }
                }

                // Gọi AddWithDetailsAsync để tạo tài khoản và thông tin bổ sung
                int result = await _accountService.AddWithDetailsAsync(newAccount, newEmployee, newAdmin);
                if (result <= 0)
                {
                    ModelState.AddModelError("", "Không thể tạo tài khoản. Email hoặc tên người dùng đã tồn tại.");
                    ViewBag.Roles = await _roleService.GetAllAsync();
                    return View(account);
                }

                TempData["SuccessMessage"] = "Tạo tài khoản thành công!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Lỗi khi tạo tài khoản: {ex.Message}");
                ViewBag.Roles = await _roleService.GetAllAsync();
                return View(account);
            }
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var account = await _accountService.GetByIdAsync(id);
            if (account == null)
            {
                return NotFound();
            }

            var roles = await _roleService.GetAllAsync();
            ViewBag.RoleId = new SelectList(roles, "RoleId", "RoleName", account.RoleId);

            return View(account);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Account account, int id)
        {
          
                int result = await _accountService.UpdateAccountAsync(account,id);

                if (result == 1)
                {
                    TempData["SuccessMessage"] = "Cập nhật tài khoản thành công!";
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "Tài khoản không tồn tại.");
                }
           

            var roles = await _roleService.GetAllAsync();
            ViewBag.Roles = roles;  
            return View(account); 
        }
        public async Task<IActionResult> Details(int id)
        {
            var account = await _accountService.GetByIdAsync(id);
            var roles = await _roleService.GetAllAsync();

            if (account == null)
            {
                return NotFound();
            }

            ViewBag.RoleName = roles.FirstOrDefault(r => r.RoleId == account.RoleId)?.RoleName ?? "Không xác định";

            return View(account);
        }

        [HttpPost]
        public async Task<IActionResult> ToggleActive(int id)
        {
            var account = await _accountService.GetByIdAsync(id);
            if (account == null)
            {
                return NotFound();
            }

            // Đảo ngược trạng thái Active
            account.Active = !(account.Active ?? false);

            await _accountService.UpdateAsync(account);

            TempData["SuccessMessage"] = "Cập nhật trạng thái thành công!";
            return RedirectToAction("Index");
        }


    }
}
