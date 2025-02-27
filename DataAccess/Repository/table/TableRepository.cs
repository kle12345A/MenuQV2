using DataAccess.Models;
using DataAccess.Repository.Base;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repository.table
{
    public class TableRepository : BaseRepository<Table>, ITableRepository
    {
        public TableRepository(MenuQContext context) : base(context)
        {
        }

        public async Task<Table> GetTableByIdAsync(int id)
        {
            return await _context.Tables.Include(t => t.Area).FirstOrDefaultAsync(t => t.TableId == id);
        }
    }
}
