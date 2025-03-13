using DataAccess.Enum;
using DataAccess.Models;
using BussinessObject.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.DTOs;

namespace BussinessObject.invoice
{
    public interface IInvoiceService : IBaseService<Invoice>
    {
        Task<List<InvoiceDTO>> GetAllInvoiceAsync();
        Task<InvoiceDetailDTO> GetInvoiceByRequestId(int requestId);
        Task<InvoiceDetailDTO> GetInvoiceByCustomer(int customerId);
        Task<ServiceResult<Invoice>> CreateInvoiceForRequest(int requestId);
        Task<ServiceResult<Invoice>> UpdateInvoiceForRequest(int requestId);
        Task<bool> UpdateInvoiceWithNewOrderDetails(int invoiceId, List<OrderDetail> newOrderDetails);
        Task<ServiceResult<Invoice>> UpdateInvoiceStatus(int requestId, InvoiceStatus status);
        Task<ServiceResult<Invoice>> Checkout(int invoiceId);
        Task<ServiceResult<Invoice>> CancelCheckout (int invoiceId);
        Task<bool> UpdatePaymentMethod(int invoiceId, string paymentMethod);
        Task<bool> ResetPaymentMethod(int invoiceId);
        Task<decimal> CalculateTotalRevenueAsync();
        Task<decimal> CalculateTotalRevenueForCurrentYearAsync();
        Task<decimal> CalculateTotalRevenueForTodayAsync();
        Task<decimal> CalculateTotalRevenueForYesterdayAsync(); // Ngày hôm qua
        Task<decimal> CalculateTotalRevenueForLastMonthAsync(); // Tháng trước
        Task<decimal> CalculateTotalRevenueForLastYearAsync(); // Năm trước
        Task<List<Invoice>> GetAllInvoicesAsync();
        // Thêm phương thức để lấy danh sách món ăn có doanh thu cao nhất
        Task<List<TopSellingItemDTO>> GetTopSellingItemsAsync(string timeRange, int topN = 5);

    }
}
