using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DTOs
{
    public class TopSellingItemDTO
    {
        public string ItemName { get; set; }
        public decimal Price { get; set; }
        public int QuantitySold { get; set; }
        public decimal TotalRevenue { get; set; }
        public string ImageUrl { get; set; }
    }
}
