using DataAccess.Enum;
using DataAccess.Models;
using DataAccess.Repository.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace DataAccess.Repository.orderdetail
{
    public class OrderDetailRepository : BaseRepository<OrderDetail>, IOrderDetailRepository
    {
        private readonly MenuQContext _context;

        public OrderDetailRepository(MenuQContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<OrderDetail>> GetOrderDetailsByRequestId(int requestId)
        {
            return await _context.OrderDetails
                .Where(od => od.RequestId == requestId)
                .Include(od => od.Item)
                    .ThenInclude(i => i.Category) // Load cả Category để hiển thị trong UI
                .AsNoTracking() // Tối ưu hiệu suất
                .ToListAsync();
        }

        public async Task<bool> UpdateOrderDetail(int orderDetailId, int newQuantity, decimal newPrice, string note)
        {
            var orderDetail = await _context.OrderDetails.FindAsync(orderDetailId);
            if (orderDetail == null) return false;

            orderDetail.Quantity = newQuantity;
            orderDetail.Price = newPrice;
            orderDetail.Note = note;

            return await SaveChanges();
        }

        public async Task<bool> DeleteOrderDetail(int orderDetailId)
        {
            var orderDetail = await _context.OrderDetails.FindAsync(orderDetailId);
            if (orderDetail == null) return false;

            _context.OrderDetails.Remove(orderDetail);
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

        public async Task<List<OrderDetail>> GetOrderDetailsByCustomerId(int customerId)
        {
            var invoice = await _context.Invoices
                .Where(i => i.CustomerId == customerId && i.InvoiceStatus == InvoiceStatus.Serving)
                .Select(i => i.RequestId) // Lấy RequestId từ hóa đơn
                .FirstOrDefaultAsync();

            if (invoice == 0) return new List<OrderDetail>(); // Không có hóa đơn Serving

            return await _context.OrderDetails
                .Where(od => od.RequestId == invoice) // Lọc theo RequestId của hóa đơn
                .Include(od => od.Item)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
