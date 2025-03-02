using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DTOs
{
    public class DashboardDTO
    {
        public decimal TotalRevenue { get; set; } 
        public decimal TotalRevenueCurrentYear { get; set; } 
        public decimal TotalRevenueToday { get; set; }
        public int CurrentYear { get; set; } 
        public string CurrentDate { get; set; } 

        // Tỷ lệ tăng giảm (%)
        public double TodayRevenueChangePercentage { get; set; } // Tăng giảm so với hôm qua
        public double MonthRevenueChangePercentage { get; set; } // Tăng giảm so với tháng trước
        public double YearRevenueChangePercentage { get; set; } // Tăng giảm so với năm trước

        // Số lượng khách hàng
        public int TotalCustomersToday { get; set; }
        public int TotalCustomersCurrentMonth { get; set; }
        public int TotalCustomersCurrentYear { get; set; }
        // Tỷ lệ tăng giảm số lượng khách hàng
        public double TodayCustomersChangePercentage { get; set; }
        public double MonthCustomersChangePercentage { get; set; }
        public double YearCustomersChangePercentage { get; set; }
    }
}
