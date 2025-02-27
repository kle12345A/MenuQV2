
using BussinessObject;
using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinessObject.menu
{
    public interface IMenuItemService : IBaseService<MenuItem>
    {
        Task<IEnumerable<MenuItem>> GetAllAsync();
        Task<int> AddAsync(MenuItem menuItemModel);
        Task<int> UpdateAsync(MenuItem menuItemModel, int id);
        Task<int> DeleteAsync(int id);

    }
}
