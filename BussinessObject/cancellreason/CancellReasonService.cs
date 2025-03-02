using Azure.Core;
using BussinessObject;
using DataAccess.Models;
using DataAccess.Repository.Base;
using DataAccess.Repository.cancellation;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace BussinessObject.cancellreason
{
    public class CancellReasonService : BaseService<CancellationReason>, ICancellReasonService
    {
        private readonly ICancellationReasonRepository _cancellReasonRepository;
        private readonly ILogger<CancellationReason> _logger;
        public CancellReasonService(IUnitOfWork unitOfWork, ICancellationReasonRepository cancellReasonRepository, ILogger<CancellationReason> logger) : base(unitOfWork)
        {
            _cancellReasonRepository = cancellReasonRepository;
            _logger = logger;
        }

        public async Task<List<CancellationReason>> GetActiveCancellationReasons()
        {
            try
            {
                return await _cancellReasonRepository.GetActiveCancellationReasons();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching cancellation reasons");
                return new List<CancellationReason>();
            }
        }

        public async Task<CancellationReason> GetCancellationReasonById(int reasonId)
        {
            try
            {
                return await _cancellReasonRepository.GetCancellationReasonById(reasonId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching cancellation reasons {reasonId}", reasonId);
                return new CancellationReason();
            }
        }
    }
}
