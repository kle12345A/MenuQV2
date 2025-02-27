using DataAccess.Models;
using DataAccess.Repository.Base;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repository.area
{
    public class AreaRepository : BaseRepository<Area>, IAreaRepository
    {
        private readonly MenuQContext _context;

        public AreaRepository(MenuQContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<Area>> GetAllWithTablesAsync()
        {
            return await _context.Areas
                .Include(a => a.Tables) // Load danh sách bàn của từng khu vực
                .ToListAsync();
        }
    }
}
