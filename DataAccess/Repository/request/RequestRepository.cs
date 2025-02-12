using DataAccess.Models;
using DataAccess.Repository.Base;

namespace DataAccess.Repository.request
{
    public class RequestRepository : BaseRepository<Request>, IRequestRepository
    {
        public RequestRepository(MenuQContext context) : base(context)
        {
        }
    }
}
