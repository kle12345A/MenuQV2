using DataAccess.Models;
using DataAccess.Repository.Base;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repository.servicereason
{
    public class ServiceReasonRepository : BaseRepository<ServiceReason>, IServiceReasonRepository
    {
        public ServiceReasonRepository(MenuQContext context) : base(context)
        {
        }

        public Task<List<ServiceReason>> GetAllActive()
        {
            return _dbSet.Where(r => r.Status == true && !r.ReasonText.Equals("DEFAULT")).ToListAsync();
        }

        public async Task<int> GetReasonDefaultId()
        {
            var reason = await _dbSet.FirstOrDefaultAsync(r => r.ReasonText == "DEFAULT");
            return reason == null ? 0 : reason.ReasonId;
        }
    }
}
