using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Enum
{
    public class PaymentMethodEnumHelper
    {
        private static readonly Dictionary<PaymentMethod, string> _paymentMethodMapping = new()
    {
        { PaymentMethod.CreditCardAtTable, "Chuyển khoản tại bàn" },
        { PaymentMethod.CreditCardAtCounter, "Chuyển khoản tại quầy thanh toán" },
        { PaymentMethod.CashAtCounter, "Thanh toán tiền mặt tại quầy thanh toán" }
    };

        public static string GetVietnameseName(PaymentMethod method)
        {
            return _paymentMethodMapping.TryGetValue(method, out var name) ? name : "Không xác định";
        }

        public static bool TryParseVietnameseName(string vietnameseName, out PaymentMethod method)
        {
            Console.WriteLine("🔍 Đang kiểm tra ánh xạ PaymentMethod từ: ");

            foreach (var kvp in _paymentMethodMapping)
            {
                if (kvp.Value.Trim().Equals(vietnameseName.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("✅ Tìm thấy ánh xạ PaymentMethod");
                    method = kvp.Key;
                    return true;
                }
            }

            Console.WriteLine("❌ Không tìm thấy ánh xạ PaymentMethod cho:");
            method = default;
            return false;
        }



    }
}
