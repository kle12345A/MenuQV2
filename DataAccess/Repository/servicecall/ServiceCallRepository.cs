using DataAccess.Models;
using DataAccess.Repository.Base;

namespace DataAccess.Repository.servicecall
{
    public class ServiceCallRepository : BaseRepository<ServiceCall>, IServiceCallRepository
    {
        public ServiceCallRepository(MenuQContext context) : base(context)
        {
        }
    }
}
