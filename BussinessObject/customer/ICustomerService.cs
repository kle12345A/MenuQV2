using BussinessObject;
using DataAccess.Models;
using MenuQ.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinessObject.customer
{
    public interface ICustomerService : IBaseService<Customer>  
    {
        Task<List<OrderHistory>> GetOrderHistoryByCustomerId(int customerId);
    }
}
