using DataAccess.Models;
using DataAccess.Repository.Base;

namespace DataAccess.Repository.menuitem
{
    public class MenuItemRepository : BaseRepository<MenuItem>, IMenuItemRepository
    {
        public MenuItemRepository(MenuQContext context) : base(context)
        {
        }
    }
}
