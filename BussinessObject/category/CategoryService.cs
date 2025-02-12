using BussinessObject;
using DataAccess.Models;
using DataAccess.Repository.Base;
using DataAccess.Repository.category;

using System;
using System.Threading.Tasks;

namespace BussinessObject.category
{
    public class CategoryService : BaseService<Category>, ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(IUnitOfWork unitOfWork, ICategoryRepository categoryRepository) : base(unitOfWork)
        {
            _categoryRepository = categoryRepository;
        }

      
    }
}
