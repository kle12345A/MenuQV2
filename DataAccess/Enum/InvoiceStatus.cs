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
        Paid = 2,      // Đã thanh toán
        Cancelled = 3  // Đã hủy
    }
}
