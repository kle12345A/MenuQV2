using DataAccess.Models;
using DataAccess.Repository.Base;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repository.cancellation
{
    public class CancellationReasonRepository : BaseRepository<CancellationReason>, ICancellationReasonRepository
    {
        private readonly MenuQContext _context;

        public CancellationReasonRepository(MenuQContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<CancellationReason>> GetActiveCancellationReasons()
        {
            return await _context.CancellationReasons
                .AsNoTracking()
                .Where(r => r.Status == true)
                .ToListAsync();
        }


        public async Task<CancellationReason> GetCancellationReasonById(int reasonId)
        {
            var reason = await _context.CancellationReasons
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.ReasonId == reasonId);

            if (reason == null)
            {
                Console.WriteLine($"Warning: CancellationReason with ID {reasonId} not found.");
            }

            return reason;
        }

    }
}
