using DataAccess.Models;
using DataAccess.Repository.Base;

namespace DataAccess.Repository.table
{
    public class TableRepository : BaseRepository<Table>, ITableRepository
    {
        public TableRepository(MenuQContext context) : base(context)
        {
        }
    }
}
