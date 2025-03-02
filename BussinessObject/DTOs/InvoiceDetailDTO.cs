using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinessObject.DTOs
{
    public class InvoiceDetailDTO
    {
        public string InvoiceCode { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CustomerName { get; set; }
        public string PhoneNumber { get; set; }
        public int? TableId { get; set; } // 🟢 Chỉ lưu TableID
        public decimal TotalAmount { get; set; }
        public string PaymentMethod { get; set; }
        public string InvoiceStatus { get; set; }
        public List<OrderDetailDTO> OrderDetails { get; set; }
    }


}
