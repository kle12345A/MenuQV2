using BussinessObject.area;
using DataAccess.Models;
using MenuQ.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace MenuQ.Areas.admin.Controllers
{
    [Area("Admin")]
    public class TableController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAreaService _areaService;

        public TableController(IAreaService areaService, IHttpContextAccessor httpContextAccessor)
        {
            _areaService = areaService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IActionResult> Index()
        {
            var areas = await _areaService.GetAllAreasWithTablesAsync();

            var areaList = areas.Select(a => new TableArea
            {
                AreaId = a.AreaId,
                AreaName = a.AreaName,
                TableCount = a.Tables?.Count() ?? 0,
                Tables = a.Tables?.Select(t => t.TableId).ToList() ?? new List<int>(),
                TableNumbers = a.Tables?.Select(t => t.TableNumber).ToList() ?? new List<string>()
            }).ToList();

            return View(areaList);
        }

        public async Task<IActionResult> GenerateQR()
        {
            var areas = await _areaService.GetAllAreasWithTablesAsync();

            var areaList = areas.Select(a => new TableArea
            {
                AreaId = a.AreaId,
                AreaName = a.AreaName,
                TableCount = a.Tables?.Count() ?? 0,
                Tables = a.Tables?.Select(t => t.TableId).ToList() ?? new List<int>(),
                TableNumbers = a.Tables?.Select(t => t.TableNumber).ToList() ?? new List<string>()
            }).ToList();

            return View(areaList);
        }

        public IActionResult RedirectToSuccessPage(int areaId, int tableNumber, string topText, string bottomText, string color, string backgroundColor)
        {
            return RedirectToAction("PrintQR", "Table", new
            {
                areaId = areaId,
                tableNumber = tableNumber,
                topText = topText,
                bottomText = bottomText,
                color = color,
                backgroundColor = backgroundColor
            });
        }
        public string GetDomain()
        {
            return _httpContextAccessor.HttpContext?.Request.Host.Value ?? "Unknown Domain";
        }

        public async Task<IActionResult> PrintQR(int areaId, int tableCount, string topText, string bottomText, string color, string backgroundColor, string selectedTables)
        {
            var tables = await _areaService.GetTablesByAreaIdAsync(areaId, tableCount);
            var area = await _areaService.GetByIdAsync(areaId);
            string domain = GetDomain();
            // Tách danh sách tên bàn từ chuỗi URL
            var tableNames = selectedTables.Split(',').Select(t => t.Trim()).ToList();
            var fullDomain = $"{Request.Scheme}://{Request.Host.Value}";
            var qrCodes = tables.Select((table, index) => new QrCodeDetails
            {
                AreaId = areaId,
                TableId = table.TableId,
                TopText = topText,
                BottomText = bottomText,
                Color = color,

                BackgroundColor = backgroundColor,

                Url = $"{fullDomain}/HomeApp/Login?tableId={table.TableId}",
                TableName = tableNames.ElementAtOrDefault(index) // Thêm tên bàn tương ứng từ danh sách

            }).ToList();

            var model = new QrCodeViewModel
            {
                AreaId = areaId,
                TableCount = tableCount,
                TopText = topText,
                AreaName = area.AreaName,
                BottomText = bottomText,
                Color = color,
                BackgroundColor = backgroundColor,
                QrCodes = qrCodes
            };

            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> AddArea([FromForm] Area area)
        {
            if (area == null || string.IsNullOrWhiteSpace(area.AreaName))
            {
                TempData["Error"] = "Tên khu vực không được để trống!";
                return RedirectToAction("Index");
            }

            try
            {
                await _areaService.AddAreaAsync(area);
                TempData["Success"] = "Thêm khu vực thành công!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi khi thêm khu vực: {ex.Message}";
            }

            return RedirectToAction("Index");
        }
    }
}