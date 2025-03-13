using BussinessObject;
using DataAccess.Models;
using DataAccess.Repository.Base;
using DataAccess.Repository.servicereason;

using System;
using System.Threading.Tasks;

namespace BussinessObject.servicereason
{
    public class ServiceReasonService : BaseService<ServiceReason>, IServiceReasonService
    {
        private readonly IServiceReasonRepository _serviceReasonRepository;

        public ServiceReasonService(IUnitOfWork unitOfWork, IServiceReasonRepository serviceReasonRepository) : base(unitOfWork)
        {
            _serviceReasonRepository = serviceReasonRepository;
        }

        public async Task<List<ServiceReason>> GetAllActive()
        {
            return await _serviceReasonRepository.GetAllActive();
        }
    }
}
