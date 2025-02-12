using DataAccess.Models;
using DataAccess.Repository.Base;

namespace DataAccess.Repository.cancellation
{
    public class CancellationReasonRepository : BaseRepository<CancellationReason>, ICancellationReasonRepository
    {
        public CancellationReasonRepository(MenuQContext context) : base(context)
        {
        }
    }
}
