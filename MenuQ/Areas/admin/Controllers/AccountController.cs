using BussinessObject.account;
using BussinessObject.role;
using DataAccess.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using X.PagedList;
namespace MenuQ.Areas.Admin.Controllers
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
