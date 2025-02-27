using DataAccess.Models;
using DataAccess.Repository.Base;
using Microsoft.EntityFrameworkCore;
using MenuQ.Models;

namespace DataAccess.Repository.customer
{
    public partial class CustomerRepository : BaseRepository<Customer>, ICustomerRepository
    {
        private readonly MenuQContext _context;

        public CustomerRepository(MenuQContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<OrderHistory>> GetOrderHistoryByCustomerId(int customerId)
        {
            var orderHistory = (from r in _context.Requests
                                join od in _context.OrderDetails on r.RequestId equals od.RequestId
                                join mi in _context.MenuItems on od.ItemId equals mi.ItemId
                                where r.CustomerId == customerId
                                orderby r.CreatedAt descending
                                select new OrderHistory
                                {
                                    RequestID = r.RequestId,
                                    OrderDate = r.CreatedAt,
                                    ItemID = od.ItemId,
                                    ItemName = mi.ItemName,
                                    Quantity = od.Quantity,
                                    Price = mi.Price
                                }).ToList();

            return orderHistory;
        }

        public async Task<Customer> getCustomerByPhone(string phoneNumber)
        {
            return await _context.Customers.FirstOrDefaultAsync(a => a.PhoneNumber == phoneNumber);
        }
    }
}
