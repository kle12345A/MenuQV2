using DataAccess.Enum;
using DataAccess.Models;
using BussinessObject.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinessObject.invoice
{
    public interface IInvoiceService : IBaseService<Invoice>
    {
        Task<List<InvoiceDTO>> GetAllAsync();
        Task<InvoiceDetailDTO> GetInvoiceByRequestId(int requestId);
        Task<Invoice> GetInvoiceByCustomer(int customerId);
        Task<ServiceResult<Invoice>> CreateInvoiceForRequest(int requestId);
        Task<ServiceResult<Invoice>> UpdateInvoiceForRequest(int requestId);
        Task<bool> UpdateInvoiceWithNewOrderDetails(int invoiceId, List<OrderDetail> newOrderDetails);
        Task<ServiceResult<Invoice>> UpdateInvoiceStatus(int requestId, InvoiceStatus status);
        Task<ServiceResult<Invoice>> Checkout(int requestId, String paymentMethod);

    }
}
