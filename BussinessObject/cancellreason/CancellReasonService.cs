using BussinessObject;
using DataAccess.Models;
using DataAccess.Repository.Base;
using DataAccess.Repository.cancellation;

using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace BussinessObject.cancellreason
{
    public class CancellReasonService : BaseService<CancellationReason>, ICancellReasonService
    {
        private readonly ICancellationReasonRepository _cancellReasonRepository;
        public CancellReasonService(IUnitOfWork unitOfWork, ICancellationReasonRepository cancellReasonRepository) : base(unitOfWork)
        {
            _cancellReasonRepository = cancellReasonRepository;
        }

      
    }
}
