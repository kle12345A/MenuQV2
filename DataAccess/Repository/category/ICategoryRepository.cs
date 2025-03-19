using DataAccess.Models;
using DataAccess.Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository.category
{
    public interface ICategoryRepository : IBaseRepository<Category>
    {
        Task<Category?> GetFirstOrDefaultAsync(Expression<Func<Category, bool>> predicate);
        Task<IEnumerable<Category>> GetAllCategoriesAsync();
    }
}
