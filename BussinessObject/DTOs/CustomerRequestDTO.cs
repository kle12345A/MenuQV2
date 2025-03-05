using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinessObject.DTOs
{
    public class CustomerRequestDTO
    {
        public int RequestId { get; set; }
        public String TableNumber { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string RequestType { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Note { get; set; } //Ghi chú từ ServiceCalls (Phương thức thanh toán)
    }
}
