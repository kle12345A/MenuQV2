using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Enum
{
    public enum InvoiceStatus
    {
        Serving = 1,   // Đang phục vụ
        ProcessingPayment = 2, // Đang thanh toán
        Paid = 3,      // Đã thanh toán
        Cancelled = 4  // Đã hủy
    }
}
