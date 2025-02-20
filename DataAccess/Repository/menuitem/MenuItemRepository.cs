using DataAccess.Models;
using DataAccess.Repository.Base;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repository.menuitem
{
    public class MenuItemRepository : BaseRepository<MenuItem>, IMenuItemRepository
    {
        public MenuItemRepository(MenuQContext context) : base(context)
        {
        }

        public IQueryable<MenuItem> GetAll()
        {
            return _context.MenuItems.Include(m => m.Category);
        }
    }
}
