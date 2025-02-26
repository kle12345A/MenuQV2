using BussinessObject.category;
using BussinessObject.file;
using BussinessObject.menu;
using DataAccess.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MenuQ.Areas.admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class MenuController : Controller
    {
        private readonly IMenuItemService _menuItemService;
        private readonly ICategoryService _categoryService;
        private readonly IFileService _fileService;
        public MenuController(IMenuItemService menuItemService, ICategoryService categoryService, IFileService fileService)
        {
            _fileService = fileService;
            _categoryService = categoryService;
            _menuItemService = menuItemService;
        }
        public async Task<IActionResult> Index()
        {
            var menu = await _menuItemService.GetAllAsync();
            return View(menu);
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


    }
}
