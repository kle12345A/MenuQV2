

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
        public async Task<Customer> GetCustomerByPhone(string Phone)
        {
            return await _customerRepository.getCustomerByPhone(Phone);
        }

        public async Task<Customer> CustomerLogin(string Phone, string username)
        {
            var customer = await _customerRepository.getCustomerByPhone(Phone);
            if (customer == null)
            {
                customer = new Customer
                {
                    CustomerName = username,
                    PhoneNumber = Phone,
                    CreatedAt = DateTime.Now,
                };
                await _customerRepository.AddAsync(customer);
            }
            return customer;
        }
    }
}
