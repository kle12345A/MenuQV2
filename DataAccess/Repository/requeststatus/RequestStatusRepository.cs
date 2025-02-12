using DataAccess.Models;
using DataAccess.Repository.Base;

namespace DataAccess.Repository.requeststatus
{
    public class RequestStatusRepository : BaseRepository<RequestStatus>, IRequestStatusRepository
    {
        public RequestStatusRepository(MenuQContext context) : base(context)
        {
        }
    }
}
