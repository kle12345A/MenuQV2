using DataAccess.Enum;
using DataAccess.Models;
using DataAccess.Repository.Base;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository.invoice
{
    public class InvoiceRepository : BaseRepository<Invoice>, IInvoiceRepository
    {
        private readonly MenuQContext _context;

        public InvoiceRepository(MenuQContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<Invoice>> GetAllInvoices()
        {
            return await _context.Invoices
                .Include(i => i.Customer)
                .Include(i => i.Request.OrderDetails)
                .ToListAsync();
        }

        public async Task<bool> CreateInvoice(Invoice invoice)
        {
            var existingInvoice = await _context.Invoices
                .AnyAsync(i => i.RequestId == invoice.RequestId);

            if (existingInvoice) return false; // Nếu đã có hóa đơn cho request này, không tạo mới.

            //Đảm bảo TableID được lưu chính xác
            var request = await _context.Requests.FindAsync(invoice.RequestId);
            if (request != null && request.TableId.HasValue)
            {
                invoice.TableId = request.TableId.Value;
            }

            await _context.Invoices.AddAsync(invoice);
            return await SaveChanges();
        }


        public async Task<Invoice?> GetInvoiceByCustomer(int customerId)
        {
            return await _context.Invoices
                .Where(i => i.CustomerId == customerId && i.InvoiceStatus == InvoiceStatus.Serving)
                .Include(i => i.Request.OrderDetails)
                .FirstOrDefaultAsync();
        }


        public async Task<Invoice> GetInvoiceByRequestId(int requestId)
        {
            return await _context.Invoices
                .Include(i => i.Customer)
                .Include(i => i.Request.OrderDetails)
                .ThenInclude(od => od.Item)
                .FirstOrDefaultAsync(i => i.RequestId == requestId);
        }


        public async Task<bool> UpdateInvoiceStatus(int invoiceId, InvoiceStatus status)
        {
            var invoice = await _context.Invoices.FindAsync(invoiceId);
            if (invoice == null) return false;

            // Chỉ cập nhật nếu invoice đang ở trạng thái "Serving"
            if (invoice.InvoiceStatus != InvoiceStatus.Serving)
            {
                return false; // Không thể cập nhật nếu invoice đã bị hủy hoặc thanh toán
            }

            invoice.InvoiceStatus = status;
            return await SaveChanges();
        }


        public async Task<bool> UpdateInvoiceTotal(int invoiceId, decimal totalAmount)
        {
            var invoice = await _context.Invoices.FindAsync(invoiceId);
            if (invoice == null) return false;

            invoice.TotalAmount = totalAmount;
            return await SaveChanges();
        }

        public async Task<bool> UpdateInvoiceWithNewOrderDetails(int invoiceId, List<OrderDetail> newOrderDetails)
        {
            var invoice = await _context.Invoices.Include(i => i.Request.OrderDetails).FirstOrDefaultAsync(i => i.InvoiceId == invoiceId);
            if (invoice == null) return false;

            foreach (var newDetail in newOrderDetails)
            {
                var existingDetail = invoice.Request.OrderDetails.FirstOrDefault(od => od.ItemId == newDetail.ItemId);
                if (existingDetail != null)
                {
                    existingDetail.Quantity += newDetail.Quantity;
                    existingDetail.Price = newDetail.Price; // Cập nhật giá nếu thay đổi
                }
                else
                {
                    invoice.Request.OrderDetails.Add(newDetail);
                }
            }

            return await SaveChanges();
        }

        public async Task<bool> SaveChanges()
        {
            try
            {
                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving changes: {ex.Message}");
                return false;
            }
        }

    }
}
