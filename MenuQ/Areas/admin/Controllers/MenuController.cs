using BussinessObject.category;
using BussinessObject.file;
using BussinessObject.menu;
using DataAccess.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using X.PagedList;

namespace MenuQ.Areas.admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class MenuController : Controller
    {
        private readonly IMenuItemService _menuItemService;
        private readonly ICategoryService _categoryService;
        private readonly IFileService _fileService;
        private const int PageSize = 5;
        public MenuController(IMenuItemService menuItemService, ICategoryService categoryService, IFileService fileService)
        {
            _fileService = fileService;
            _categoryService = categoryService;
            _menuItemService = menuItemService;
        }
        public async Task<IActionResult> Index(int? page = 1)
        {
            int pageNumber = page ?? 1;
            int pageSize = PageSize;

            var menuItems = await _menuItemService.GetAllAsync();
            var pagedMenuItems = menuItems.ToPagedList(pageNumber, pageSize);

            return View(pagedMenuItems);
        }

        // Action mới để xử lý tìm kiếm AJAX
        [HttpGet]
        public async Task<IActionResult> SearchMenuItems(string search, int page = 1)
        {
            int pageSize = PageSize;

            var menuItems = await _menuItemService.GetAllAsync();

            // Lọc danh sách nếu có từ khóa tìm kiếm
            if (!string.IsNullOrEmpty(search))
            {
                menuItems = menuItems
                    .Where(m => m.ItemName.ToLower().Contains(search.ToLower()) ||
                                (m.Category != null && m.Category.CategoryName.ToLower().Contains(search.ToLower())) ||
                                (m.Status == true ? "Có sẵn" : "Hết hàng").ToLower().Contains(search.ToLower()))
                    .ToList();
            }

            var pagedMenuItems = menuItems.ToPagedList(page, pageSize);

            // Chuẩn bị dữ liệu để trả về dưới dạng JSON
            var result = new
            {
                items = pagedMenuItems.Select(item => new
                {
                    itemId = item.ItemId,
                    itemName = item.ItemName,
                    price = item.Price,
                    imageUrl = item.ImageUrl,
                    status = item.Status,
                    categoryName = item.Category?.CategoryName ?? "Không có danh mục"
                }).ToList(),
                hasNextPage = pagedMenuItems.HasNextPage,
                hasPreviousPage = pagedMenuItems.HasPreviousPage,
                pageNumber = pagedMenuItems.PageNumber,
                pageCount = pagedMenuItems.PageCount,
                totalItems = pagedMenuItems.TotalItemCount
            };

            return Json(result);
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var categories = await _categoryService.GetAllAsync();

            ViewBag.Categories = categories.Select(c => new SelectListItem
            {
                Value = c.CategoryId.ToString(),
                Text = c.CategoryName
            }).ToList();

            return View(new MenuItem());
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MenuItem menuItemModel, IFormFile ImageFile)
        {
            if (!ModelState.IsValid)
            {
                Console.WriteLine("ImageFile: " + menuItemModel.ImageUrl);
                var categories = await _categoryService.GetAllAsync();
                ViewBag.Categories = categories.Select(c => new SelectListItem
                {
                    Value = c.CategoryId.ToString(),
                    Text = c.CategoryName
                }).ToList();
                return View(menuItemModel);
            }
            

            try
            {
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    string imageUrl = await _fileService.UploadImageAsync(ImageFile, "Menu");
                    menuItemModel.ImageUrl = imageUrl;
                }
                else
                {
                    ViewData["ImageError"] = "Vui lòng chọn ảnh hợp lệ.";
                    return View(menuItemModel);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Lỗi khi tải ảnh lên: " + ex.Message);
                return View(menuItemModel);
            }

            int result = await _menuItemService.AddAsync(menuItemModel);
            if (result > 0)
            {
                TempData["SuccessMessage"] = "Thêm món ăn thành công!";
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", "Có lỗi xảy ra khi thêm món ăn.");
            return View(menuItemModel);
        }


        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var menuItem = await _menuItemService.GetByIdAsync(id);
            if (menuItem == null)
            {
                return NotFound();
            }

            var categories = await _categoryService.GetAllAsync();
            ViewBag.Categories = categories.Select(c => new SelectListItem
            {
                Value = c.CategoryId.ToString(),
                Text = c.CategoryName
            }).ToList();

            return View(menuItem);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(MenuItem menuItemModel, IFormFile? ImageFile)
        {
            if (!ModelState.IsValid)
            {
                var categories = await _categoryService.GetAllAsync();
                ViewBag.Categories = categories.Select(c => new SelectListItem
                {
                    Value = c.CategoryId.ToString(),
                    Text = c.CategoryName
                }).ToList();
                return View(menuItemModel);
            }

            try
            {
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    string imageUrl = await _fileService.UploadImageAsync(ImageFile, "Menu");
                    menuItemModel.ImageUrl = imageUrl;
                }
                else
                {
                    var existingMenuItem = await _menuItemService.GetByIdAsync(menuItemModel.ItemId);
                    menuItemModel.ImageUrl = existingMenuItem?.ImageUrl;
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Lỗi khi tải ảnh lên: " + ex.Message);
                return View(menuItemModel);
            }

            int result = await _menuItemService.UpdateAsync(menuItemModel);
            if (result > 0)
            {
                TempData["SuccessMessage"] = "Cập nhật món ăn thành công!";
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", "Có lỗi xảy ra khi cập nhật món ăn.");
            return View(menuItemModel);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var menuItem = await _menuItemService.GetByIdAsync(id);
            if (menuItem == null)
            {
                return NotFound();
            }

            return View(menuItem);
        }
        public IActionResult DownloadTemplate()
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/templates/menu-template.xlsx");
            var mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            return PhysicalFile(filePath, mimeType, "menu-template.xlsx");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _menuItemService.DeleteAsync(id);
            if (result > 0)
            {
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }



        [HttpPost]
        public async Task<IActionResult> ImportMenu(IFormFile excelFile)
        {
            if (excelFile != null && excelFile.Length > 0)
            {
                using (var stream = excelFile.OpenReadStream())
                {
                    await _menuItemService.ImportMenuItemsFromExcelAsync(stream);
                }
                TempData["SuccessMessage"] = "Import thành công!";
                return RedirectToAction("Index");
            }

            TempData["ErrorMessage"] = "Vui lòng chọn file Excel hợp lệ.";
            return RedirectToAction("Index");
        }




    }
}
