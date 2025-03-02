using BussinessObject;
using DataAccess.Models;
using DataAccess.Repository.Base;
using DataAccess.Repository.requeststatus;
using System;
using System.Threading.Tasks;

namespace BussinessObject.requeststatus
{
    public class RequestStatusService : BaseService<RequestStatus>, IRequestStatusService
    {
        private readonly IRequestStatusRepository _requestStatusRepository;

        public RequestStatusService(IUnitOfWork unitOfWork, IRequestStatusRepository requestStatusRepository) : base(unitOfWork)
        {
            _requestStatusRepository = requestStatusRepository;
        }
    }
}
