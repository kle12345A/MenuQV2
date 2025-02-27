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

        // Lấy tất cả menu items
        public async Task<IEnumerable<MenuItem>> GetAllAsync()
        {
            return await _menuItemRepository.GetAllAsync();
        }
        // Thêm menu item
        public async Task<int> AddAsync(MenuItem menuItemModel)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var newMenuItem = new MenuItem
                {
                    CategoryId = menuItemModel.CategoryId,
                    ItemName = menuItemModel.ItemName,
                    Descriptions = menuItemModel.Descriptions,
                    Price = menuItemModel.Price,
                    ImageUrl = menuItemModel.ImageUrl,
                    Status = menuItemModel.Status ?? true,
                };

                await _menuItemRepository.AddAsync(newMenuItem);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                return 1;
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        // Cập nhật menu item
        public async Task<int> UpdateAsync(MenuItem menuItemModel, int id)
        {
            var existingMenuItem = await _menuItemRepository.GetByIdAsync(id);
            if (existingMenuItem == null)
            {
                return -1;
            }

            existingMenuItem.CategoryId = menuItemModel.CategoryId;
            existingMenuItem.ItemName = menuItemModel.ItemName;
            existingMenuItem.Descriptions = menuItemModel.Descriptions;
            existingMenuItem.Price = menuItemModel.Price;
            existingMenuItem.ImageUrl = menuItemModel.ImageUrl;
            existingMenuItem.Status = menuItemModel.Status ?? existingMenuItem.Status;

            await _menuItemRepository.UpdateAsync(existingMenuItem);
            await _unitOfWork.SaveChangesAsync();

            return 1;
        }

        // Xóa menu item
        public async Task<int> DeleteAsync(int id)
        {
            var existingMenuItem = await _menuItemRepository.GetByIdAsync(id);
            if (existingMenuItem == null)
            {
                return -1;
            }

            await _menuItemRepository.DeleteAsync(existingMenuItem);
            await _unitOfWork.SaveChangesAsync();

            return 1;
        }


    }
}
