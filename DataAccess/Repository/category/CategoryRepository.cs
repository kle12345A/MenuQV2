using DataAccess.Models;
using DataAccess.Repository.Base;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DataAccess.Repository.category
{
    public class CategoryRepository : BaseRepository<Category>, ICategoryRepository
    {
        private readonly DbContext _dbContext;
        public CategoryRepository(MenuQContext context) : base(context)
        {
            _dbContext =  context;
        }

        public async Task<Category?> GetFirstOrDefaultAsync(Expression<Func<Category, bool>> predicate)
        {
            return await _dbContext.Set<Category>().FirstOrDefaultAsync(predicate);
        }
    }
}
