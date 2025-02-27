using BussinessObject;
using DataAccess.Models;
using DataAccess.Repository.Base;
using DataAccess.Repository.menuitem;
using Microsoft.EntityFrameworkCore;
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
        //===========================================================
        // Lấy tất cả menu items
        public async Task<IEnumerable<MenuItem>> GetAllAsync()
        {
            return await _menuItemRepository.GetAllModelAsync();
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
        public async Task<IEnumerable<MenuItem>> GetAllAsync()
        {
            return await _menuItemRepository.GetAll()
           .Include(m => m.Category)
           .ToListAsync();
        }

        public async Task<int> AddAsync(MenuItem menuItemModel)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var newMenuItem = new MenuItem
                {
                    ItemId = menuItemModel.ItemId,
                    CategoryId = menuItemModel.CategoryId,
                    ItemName = menuItemModel.ItemName,
                    Descriptions = menuItemModel.Descriptions,
                    Price = menuItemModel.Price,
                    ImageUrl = menuItemModel.ImageUrl,
                    Status = menuItemModel.Status ?? true,
                    IsHot = menuItemModel.IsHot,
                    IsNew = menuItemModel.IsNew
                };

                return 1;
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        // Cập nhật menu item
        public async Task<int> UpdateModelAsync(MenuItem menuItemModel, int id)
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

//Kien=============================================
                await _menuItemRepository.AddAsync(newMenuItem);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                return 1; // Trả về 1 nếu thành công
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task<int> UpdateAsync(MenuItem menuItemModel)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var existingMenuItem = await _menuItemRepository.GetAll()
                    .FirstOrDefaultAsync(m => m.ItemId == menuItemModel.ItemId);

                if (existingMenuItem == null)
                {
                    return 0; // Không tìm thấy món ăn
                }

                // Cập nhật thông tin món ăn
                existingMenuItem.CategoryId = menuItemModel.CategoryId;
                existingMenuItem.ItemName = menuItemModel.ItemName;
                existingMenuItem.Descriptions = menuItemModel.Descriptions;
                existingMenuItem.Price = menuItemModel.Price;
                existingMenuItem.ImageUrl = menuItemModel.ImageUrl ?? existingMenuItem.ImageUrl; // Giữ nguyên ảnh cũ nếu không có ảnh mới
                existingMenuItem.Status = menuItemModel.Status ?? existingMenuItem.Status;
                existingMenuItem.IsHot = menuItemModel.IsHot;
                existingMenuItem.IsNew = menuItemModel.IsNew;

               await _menuItemRepository.UpdateAsync(existingMenuItem);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                return 1; // Thành công
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

    }
}
