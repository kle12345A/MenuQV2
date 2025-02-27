using DataAccess.Models;
using DataAccess.Repository.Base;

namespace DataAccess.Repository.menuitem
{
    public class MenuItemRepository : BaseRepository<MenuItem>, IMenuItemRepository
    {
        public MenuItemRepository(MenuQContext context) : base(context)
        {
        }

        public Customer GetCustomerByPhone(string phoneNumber)
        {
            throw new NotImplementedException();
        }

        public IQueryable<MenuItem> GetAll()
        {
            return _context.MenuItems.Include(m => m.Category);
        }
    }
}
