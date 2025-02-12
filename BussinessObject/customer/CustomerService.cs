

using BussinessObject;
using DataAccess.Models;
using DataAccess.Repository.Base;
using DataAccess.Repository.customer;

namespace BussinessObject.customer
{
    public class CustomerService : BaseService<Customer>, ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomerService(IUnitOfWork unitOfWork, ICustomerRepository customerRepository) : base(unitOfWork)
        {
            _customerRepository = customerRepository;
        }

    }
}
