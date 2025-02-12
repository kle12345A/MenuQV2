using DataAccess.Models;
using DataAccess.Repository.Base;

namespace DataAccess.Repository.requesttype
{
    public class RequestTypeRepository : BaseRepository<RequestType>, IRequestTypeRepository
    {
        public RequestTypeRepository(MenuQContext context) : base(context)
        {
        }
    }
}
