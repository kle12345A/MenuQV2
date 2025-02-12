using DataAccess.Models;
using DataAccess.Repository.Base;

namespace DataAccess.Repository.servicereason
{
    public class ServiceReasonRepository : BaseRepository<ServiceReason>, IServiceReasonRepository
    {
        public ServiceReasonRepository(MenuQContext context) : base(context)
        {
        }
    }
}
