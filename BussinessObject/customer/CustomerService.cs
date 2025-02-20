

using BussinessObject;
using DataAccess.Models;
using DataAccess.Repository.Base;
using DataAccess.Repository.customer;
using MenuQ.Models;

namespace BussinessObject.customer
{
    public class CustomerService : BaseService<Customer>, ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomerService(IUnitOfWork unitOfWork, ICustomerRepository customerRepository) : base(unitOfWork)
        {
            _customerRepository = customerRepository;
        }

        public async Task<List<OrderHistory>> GetOrderHistoryByCustomerId(int customerId)
        {
            return await _customerRepository.GetOrderHistoryByCustomerId(customerId);
        }
    }
}
