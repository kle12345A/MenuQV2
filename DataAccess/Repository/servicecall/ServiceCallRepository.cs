using DataAccess.Models;
using DataAccess.Repository.Base;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repository.servicecall
{
    public class ServiceCallRepository : BaseRepository<ServiceCall>, IServiceCallRepository
    {
        private readonly MenuQContext _context;

        public ServiceCallRepository(MenuQContext context) : base(context)
        {
            _context = context;
        }

        public async Task<bool> AddServiceCall(ServiceCall serviceCall)
        {
            try
            {
                await _context.ServiceCalls.AddAsync(serviceCall);
                return await SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error adding service call.");
                return false;
            }
        }

        public async Task<ServiceCall?> GetServiceCallWithRequestId(int requestId)
        {
            return await _context.ServiceCalls
                .Where(sc => sc.RequestId == requestId)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> SaveChanges()
        {
            try
            {
                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] SaveChanges failed: {ex.Message}");
                return false;
            }
        }
    }
}
