using DataAccess.Models;
using DataAccess.Repository.Base;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccess.Repository.orderdetail
{
    public class OrderDetailRepository : BaseRepository<OrderDetail>, IOrderDetailRepository
    {
        private readonly MenuQContext _context;
        public OrderDetailRepository(MenuQContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<OrderDetail>> GetAllWithDetailsAsync()
        {
            return await _context.OrderDetails
                .Include(od => od.Request)
                .Include(od => od.Item)
                .ToListAsync();
        }
    }
}
