using BussinessObject.invoice;
using BussinessObject.customer;
using DataAccess.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace MenuQ.Areas.admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class DashboardController : Controller
    {
        private readonly IInvoiceService _invoiceService;
        private readonly ICustomerService _customerService;

        public DashboardController(IInvoiceService invoiceService, ICustomerService customerService)
        {
            _invoiceService = invoiceService;
            _customerService = customerService;
        }

        public async Task<IActionResult> Index(string filter = "This Month", string customerFilter = "This Year", string topSellingFilter = "Today")
        {
            // Tính doanh thu các khoảng thời gian
            var totalRevenueToday = await _invoiceService.CalculateTotalRevenueForTodayAsync();
            var totalRevenueYesterday = await _invoiceService.CalculateTotalRevenueForYesterdayAsync();
            var totalRevenueCurrentMonth = await _invoiceService.CalculateTotalRevenueAsync();
            var totalRevenueLastMonth = await _invoiceService.CalculateTotalRevenueForLastMonthAsync();
            var totalRevenueCurrentYear = await _invoiceService.CalculateTotalRevenueForCurrentYearAsync();
            var totalRevenueLastYear = await _invoiceService.CalculateTotalRevenueForLastYearAsync();

            // Tính tỷ lệ tăng giảm doanh thu (%)
            double todayRevenueChangePercentage = CalculateChangePercentage(totalRevenueToday, totalRevenueYesterday);
            double monthRevenueChangePercentage = CalculateChangePercentage(totalRevenueCurrentMonth, totalRevenueLastMonth);
            double yearRevenueChangePercentage = CalculateChangePercentage(totalRevenueCurrentYear, totalRevenueLastYear);

            // Tính số lượng khách hàng các khoảng thời gian
            var totalCustomersToday = await _customerService.CalculateTotalCustomersForTodayAsync();
            var totalCustomersYesterday = await _customerService.CalculateTotalCustomersForYesterdayAsync();
            var totalCustomersCurrentMonth = await _customerService.CalculateTotalCustomersForCurrentMonthAsync();
            var totalCustomersLastMonth = await _customerService.CalculateTotalCustomersForLastMonthAsync();
            var totalCustomersCurrentYear = await _customerService.CalculateTotalCustomersForCurrentYearAsync();
            var totalCustomersLastYear = await _customerService.CalculateTotalCustomersForLastYearAsync();

            // Tính tỷ lệ tăng giảm số lượng khách hàng (%)
            double todayCustomersChangePercentage = CalculateChangePercentage(totalCustomersToday, totalCustomersYesterday);
            double monthCustomersChangePercentage = CalculateChangePercentage(totalCustomersCurrentMonth, totalCustomersLastMonth);
            double yearCustomersChangePercentage = CalculateChangePercentage(totalCustomersCurrentYear, totalCustomersLastYear);

            // Quyết định doanh thu hiển thị dựa trên filter (Revenue Card)
            decimal selectedRevenue;
            string filterLabel;
            double selectedRevenueChangePercentage;
            switch (filter)
            {
                case "Today":
                    selectedRevenue = totalRevenueToday;
                    filterLabel = "Today";
                    selectedRevenueChangePercentage = todayRevenueChangePercentage;
                    break;
                case "This Year":
                    selectedRevenue = totalRevenueCurrentYear;
                    filterLabel = "This Year";
                    selectedRevenueChangePercentage = yearRevenueChangePercentage;
                    break;
                case "This Month":
                default:
                    selectedRevenue = totalRevenueCurrentMonth;
                    filterLabel = "This Month";
                    selectedRevenueChangePercentage = monthRevenueChangePercentage;
                    break;
            }

            // Quyết định số lượng khách hàng hiển thị dựa trên customerFilter (Customers Card)
            int selectedCustomers;
            string customerFilterLabel;
            double selectedCustomersChangePercentage;
            switch (customerFilter)
            {
                case "Today":
                    selectedCustomers = totalCustomersToday;
                    customerFilterLabel = "Today";
                    selectedCustomersChangePercentage = todayCustomersChangePercentage;
                    break;
                case "This Month":
                    selectedCustomers = totalCustomersCurrentMonth;
                    customerFilterLabel = "This Month";
                    selectedCustomersChangePercentage = monthCustomersChangePercentage;
                    break;
                case "This Year":
                default:
                    selectedCustomers = totalCustomersCurrentYear;
                    customerFilterLabel = "This Year";
                    selectedCustomersChangePercentage = yearCustomersChangePercentage;
                    break;
            }

            // Tính danh sách Top Selling dựa trên topSellingFilter
            var topSellingItems = await _invoiceService.GetTopSellingItemsAsync(topSellingFilter);

            // Tạo đối tượng DashboardDTO
            var dash = new DashboardDTO
            {
                TotalRevenue = selectedRevenue,
                TotalRevenueCurrentYear = totalRevenueCurrentYear,
                TotalRevenueToday = totalRevenueToday,
                CurrentYear = DateTime.UtcNow.Year,
                CurrentDate = DateTime.UtcNow.ToString("yyyy-MM-dd"),
                TodayRevenueChangePercentage = todayRevenueChangePercentage,
                MonthRevenueChangePercentage = monthRevenueChangePercentage,
                YearRevenueChangePercentage = yearRevenueChangePercentage,
                TotalCustomersToday = totalCustomersToday,
                TotalCustomersCurrentMonth = totalCustomersCurrentMonth,
                TotalCustomersCurrentYear = totalCustomersCurrentYear,
                TodayCustomersChangePercentage = todayCustomersChangePercentage,
                MonthCustomersChangePercentage = monthCustomersChangePercentage,
                YearCustomersChangePercentage = yearCustomersChangePercentage
            };

            ViewBag.FilterLabel = filterLabel;
            ViewBag.SelectedChangePercentage = selectedRevenueChangePercentage;
            ViewBag.CustomerFilter = customerFilter;
            ViewBag.CustomerFilterLabel = customerFilterLabel;
            ViewBag.CustomerChangePercentage = selectedCustomersChangePercentage;
            ViewBag.TopSellingFilter = topSellingFilter;
            ViewBag.TopSellingItems = topSellingItems; // Truyền danh sách Top Selling qua ViewBag
            return View(dash);
        }

        // Phương thức tiện ích để tính tỷ lệ tăng giảm (%) (dùng cho cả doanh thu và khách hàng)
        private double CalculateChangePercentage(decimal current, decimal previous)
        {
            if (previous == 0) return current > 0 ? 100 : 0;
            return (double)((current - previous) / previous * 100);
        }

        // Phương thức tiện ích để tính tỷ lệ tăng giảm cho số lượng khách hàng (int)
        private double CalculateChangePercentage(int current, int previous)
        {
            if (previous == 0) return current > 0 ? 100 : 0;
            return (double)((current - previous) / previous * 100);
        }

        [HttpGet]
        public async Task<IActionResult> GetChartData(string reportFilter)
        {
            var invoices = await _invoiceService.GetAllInvoicesAsync();
            var customers = await _customerService.GetAllAsync();

            List<decimal> revenueData = new List<decimal>();
            List<int> customersData = new List<int>();
            List<string> timeCategories = new List<string>();

            switch (reportFilter)
            {
                case "Today":
                    var today = DateTime.UtcNow.Date;
                    for (int hour = 0; hour < 24; hour++)
                    {
                        var startHour = today.AddHours(hour);
                        var endHour = startHour.AddHours(1);

                        var revenueInHour = invoices
                            .Where(i => i.CreatedAt >= startHour && i.CreatedAt < endHour && i.InvoiceStatus == DataAccess.Enum.InvoiceStatus.Paid)
                            .Sum(i => i.TotalAmount);
                        var customersInHour = customers
                            .Count(c => c.CreatedAt.HasValue && c.CreatedAt.Value >= startHour && c.CreatedAt.Value < endHour);

                        revenueData.Add(revenueInHour);
                        customersData.Add(customersInHour);
                        timeCategories.Add(startHour.ToString("yyyy-MM-dd HH:mm:ss"));
                    }
                    break;

                case "This Month":
                    var currentMonthStart = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
                    var daysInMonth = DateTime.DaysInMonth(DateTime.UtcNow.Year, DateTime.UtcNow.Month);
                    for (int day = 1; day <= daysInMonth; day++)
                    {
                        var date = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, day);
                        var nextDate = date.AddDays(1);

                        var revenueInDay = invoices
                            .Where(i => i.CreatedAt >= date && i.CreatedAt < nextDate && i.InvoiceStatus == DataAccess.Enum.InvoiceStatus.Paid)
                            .Sum(i => i.TotalAmount);
                        var customersInDay = customers
                            .Count(c => c.CreatedAt.HasValue && c.CreatedAt.Value >= date && c.CreatedAt.Value < nextDate);

                        revenueData.Add(revenueInDay);
                        customersData.Add(customersInDay);
                        timeCategories.Add(date.ToString("yyyy-MM-dd"));
                    }
                    break;

                case "This Year":
                default:
                    var currentYear = DateTime.UtcNow.Year;
                    for (int month = 1; month <= 12; month++)
                    {
                        var startMonth = new DateTime(currentYear, month, 1);
                        var endMonth = startMonth.AddMonths(1);

                        var revenueInMonth = invoices
                            .Where(i => i.CreatedAt >= startMonth && i.CreatedAt < endMonth && i.InvoiceStatus == DataAccess.Enum.InvoiceStatus.Paid)
                            .Sum(i => i.TotalAmount);
                        var customersInMonth = customers
                            .Count(c => c.CreatedAt.HasValue && c.CreatedAt.Value >= startMonth && c.CreatedAt.Value < endMonth);

                        revenueData.Add(revenueInMonth);
                        customersData.Add(customersInMonth);
                        timeCategories.Add(startMonth.ToString("yyyy-MM"));
                    }
                    break;
            }

            return Json(new { revenueData, customersData, timeCategories });
        }
    }
}