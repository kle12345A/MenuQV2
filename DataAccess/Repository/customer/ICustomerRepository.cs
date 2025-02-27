using DataAccess.Models;
using DataAccess.Repository.Base;
using MenuQ.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository.customer
{
    public interface ICustomerRepository : IBaseRepository<Customer>
    {
        Task<List<OrderHistory>> GetOrderHistoryByCustomerId(int customerId);

        Task<Customer> getCustomerByPhone(string phoneNumber);
    }
}
