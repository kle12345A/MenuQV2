using DataAccess.Models;
using DataAccess.Repository.Base;

namespace DataAccess.Repository.operatinghour
{
    public class OperatingHourRepository : BaseRepository<OperatingHour>, IOperatingHourRepository
    {
        public OperatingHourRepository(MenuQContext context) : base(context)
        {
        }
    }
}
