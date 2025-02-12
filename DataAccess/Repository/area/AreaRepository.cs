using DataAccess.Models;
using DataAccess.Repository.Base;

namespace DataAccess.Repository.area
{
    public class AreaRepository : BaseRepository<Area>, IAreaRepository
    {
        public AreaRepository(MenuQContext context) : base(context)
        {
        }
    }
}
