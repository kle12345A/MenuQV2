using System.Linq;
using System.Threading.Tasks;
using BussinessObject;
using DataAccess.Models;
using DataAccess.Repository.Base;
using DataAccess.Repository.category;

namespace BussinessObject.category
{
    public class CategoryService : BaseService<Category>, ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(IUnitOfWork unitOfWork, ICategoryRepository categoryRepository)
            : base(unitOfWork)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<Category> AddCategory(Category category)
        {
            if (category == null || string.IsNullOrWhiteSpace(category.CategoryName))
            {
                throw new ArgumentException("Invalid category data");
            }

            // Kiểm tra xem danh mục đã tồn tại chưa
            var existingCategory = await _categoryRepository
                .GetFirstOrDefaultAsync(c => c.CategoryName.ToLower() == category.CategoryName.ToLower());

            if (existingCategory != null)
            {
                throw new InvalidOperationException("Category already exists");
            }

            // Nếu chưa tồn tại, thêm mới vào DB
            await _categoryRepository.AddAsync(category);
            await _categoryRepository.SaveChangesAsync();

            return category;
        }

        public async Task<IEnumerable<Category>> GetAllCategories()
        {
            return await _categoryRepository.GetAllCategoriesAsync();
        }
    }
}
