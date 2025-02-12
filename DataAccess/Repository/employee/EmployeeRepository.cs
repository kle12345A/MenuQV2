using DataAccess.Models;
using DataAccess.Repository.Base;

namespace DataAccess.Repository.employee
{
    public class EmployeeRepository : BaseRepository<Employee>, IEmployeeRepository
    {
        public EmployeeRepository(MenuQContext context) : base(context)
        {
        }
    }
}
