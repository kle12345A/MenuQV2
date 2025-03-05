using DataAccess.Enum;
using DataAccess.Models;
using DataAccess.Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository.invoice
{
    public interface IInvoiceRepository : IBaseRepository<Invoice>
    {
        Task<List<Invoice>> GetAllInvoices();
        //Task<Invoice> GetInvoiceById(int invoiceId);
        Task<Invoice> GetInvoiceByRequestId(int requestId);
        Task<Invoice> GetInvoiceByCustomer(int customerId);
        Task<bool> UpdateInvoiceWithNewOrderDetails(int invoiceId, List<OrderDetail> newOrderDetails);

        Task<bool> CreateInvoice(Invoice invoice);
        Task<bool> UpdateInvoiceTotal(int invoiceId, decimal totalAmount);
        Task<bool> UpdateInvoiceStatus(int invoiceId, InvoiceStatus status);
        Task<bool> UpdatePaymentMethod(int invoiceId, PaymentMethod paymentMethod);
        Task<bool> UpdateInvoice(Invoice invoice);

        Task<bool> SaveChanges();
    }
}
