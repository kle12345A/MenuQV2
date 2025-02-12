
using BussinessObject;
using DataAccess.Models;
using DataAccess.Repository.Base;
using DataAccess.Repository.employee;
using System;
using System.Threading.Tasks;

namespace BussinessObject.employee
{
    public class EmployeeService : BaseService<Employee>, IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;

        public EmployeeService(IUnitOfWork unitOfWork, IEmployeeRepository employeeRepository) : base(unitOfWork)
        {
            _employeeRepository = employeeRepository;
        }

        
    }
}
