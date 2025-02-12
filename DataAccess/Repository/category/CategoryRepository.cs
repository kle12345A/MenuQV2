using DataAccess.Models;
using DataAccess.Repository.Base;

namespace DataAccess.Repository.category
{
    public class CategoryRepository : BaseRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(MenuQContext context) : base(context)
        {
        }
    }
}
