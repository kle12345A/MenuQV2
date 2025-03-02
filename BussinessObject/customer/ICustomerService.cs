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
        Task<Customer> GetCustomerByPhone(string Phone);
        Task<List<OrderHistory>> GetOrderHistoryByCustomerId(int customerId);

        Task<int> CalculateTotalCustomersForTodayAsync(); 
        Task<int> CalculateTotalCustomersForCurrentMonthAsync(); 
        Task<int> CalculateTotalCustomersForCurrentYearAsync(); 
        Task<int> CalculateTotalCustomersForYesterdayAsync();
        Task<int> CalculateTotalCustomersForLastMonthAsync(); 
        Task<int> CalculateTotalCustomersForLastYearAsync();

        Task<Customer> CustomerLogin(string Phone, string username);

    }
}
