using DataAccess.Models;
using DataAccess.Repository.Base;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repository.employee
{
    public class EmployeeRepository : BaseRepository<Employee>, IEmployeeRepository
    {
        private MenuQContext menuQContext;
        public EmployeeRepository(MenuQContext context) : base(context)
        {
            menuQContext = context;
        }

        public IQueryable<Employee> GetAll()
        {
            return menuQContext.Employees.Include(a => a.Account);
        }
    }
}
