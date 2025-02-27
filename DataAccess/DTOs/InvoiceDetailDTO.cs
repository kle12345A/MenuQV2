using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DTOs
{
    public class InvoiceDetailDTO
    {
        public string InvoiceCode { get; set; }
        public string CustomerName { get; set; }
        public string PhoneNumber { get; set; }
        public string TableName { get; set; }
        public decimal TotalAmount { get; set; }
        public string PaymentMethod { get; set; }
        public string InvoiceStatus { get; set; }
        public List<OrderDetailDTO> OrderDetails { get; set; }
    }
}
