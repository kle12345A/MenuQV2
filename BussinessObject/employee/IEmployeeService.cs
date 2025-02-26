using BussinessObject;
using DataAccess.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinessObject.employee
{
    public interface IEmployeeService : IBaseService<Employee>
    {
        Task<IEnumerable<Employee>> GetAllEmployee();
        Task<int> AddAsync(Employee employee);
        Task<int> UpdateAsync(Employee employee);

    }
}
