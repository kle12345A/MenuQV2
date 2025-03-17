using BussinessObject;
using DataAccess.Models;
using DataAccess.Repository.Base;
using DataAccess.Repository.menuitem;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using LicenseContext = OfficeOpenXml.LicenseContext;

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
            return await _menuItemRepository.GetAll()
           .Include(m => m.Category)
           .ToListAsync();
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

                return 1; // Trả về 1 nếu thành công
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

        public async Task<int> ImportMenuItemsFromExcelAsync(Stream fileStream)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var package = new ExcelPackage(fileStream))
            {
                var worksheet = package.Workbook.Worksheets[0];
                int rowCount = worksheet.Dimension.Rows;

                await _unitOfWork.BeginTransactionAsync();

                try
                {
                    for (int row = 2; row <= rowCount; row++)
                    {
                        var menuItem = new MenuItem
                        {
                            CategoryId = Convert.ToInt32(worksheet.Cells[row, 1].Value),
                            ItemName = worksheet.Cells[row, 2].Value?.ToString(),
                            Descriptions = worksheet.Cells[row, 3].Value?.ToString(),
                            Price = Convert.ToDecimal(worksheet.Cells[row, 4].Value),
                            ImageUrl = worksheet.Cells[row, 5].Value?.ToString(),
                            Status = Convert.ToBoolean(Convert.ToInt32(worksheet.Cells[row, 6].Value)),
                            IsHot = Convert.ToBoolean(Convert.ToInt32(worksheet.Cells[row, 7].Value)),
                            IsNew = Convert.ToBoolean(Convert.ToInt32(worksheet.Cells[row, 8].Value))
                        };

                        await _menuItemRepository.AddAsync(menuItem);
                    }

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

        public async Task<IEnumerable<MenuItem>> GetAllMenuAsync()
        {
            return await _menuItemRepository.GetAll().Where(m => m.Status == true)
           .Include(m => m.Category)
           .ToListAsync();
        }
    }
}
