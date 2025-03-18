using BussinessObject;
using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinessObject.category
{
    public interface ICategoryService : IBaseService<Category>  
    {
        Task<Category> AddCategory(Category category);
    }
}
