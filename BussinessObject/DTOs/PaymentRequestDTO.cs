using DataAccess.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinessObject.DTOs
{
    public class PaymentRequestDTO
    {
        public int CustomerId { get; set; }
        public int TableId { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
    }
}
