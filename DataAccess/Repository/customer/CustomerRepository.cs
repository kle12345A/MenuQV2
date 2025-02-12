using DataAccess.Models;
using DataAccess.Repository.Base;

namespace DataAccess.Repository.customer
{
    public partial class CustomerRepository : BaseRepository<Customer>, ICustomerRepository
    {
        public CustomerRepository(MenuQContext context) : base(context)
        {

        }

    }
}
