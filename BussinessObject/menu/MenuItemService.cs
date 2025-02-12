using BussinessObject;
using DataAccess.Models;
using DataAccess.Repository.Base;
using DataAccess.Repository.menuitem;

using System;
using System.Threading.Tasks;

namespace BussinessObject.menu
{
    public class MenuItemService : BaseService<MenuItem>, IMenuItemService
    {
        private readonly IMenuItemRepository _menuItemRepository;

        public MenuItemService(IUnitOfWork unitOfWork, IMenuItemRepository menuItemRepository) : base(unitOfWork)
        {
            _menuItemRepository = menuItemRepository;
        }

       

    }
}
