using DataAccess.Models;
using DataAccess.Repository.Base;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repository.customer
{
    public partial class CustomerRepository : BaseRepository<Customer>, ICustomerRepository
    {
        private readonly MenuQContext _context;
        public CustomerRepository(MenuQContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Customer> getCustomerByPhone(string phoneNumber)
        {
            return await _context.Customers.FirstOrDefaultAsync(a => a.PhoneNumber == phoneNumber);
        }
    }
}
