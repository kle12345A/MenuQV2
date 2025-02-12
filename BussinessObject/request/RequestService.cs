using BussinessObject;
using DataAccess.Models;
using DataAccess.Repository.Base;
using DataAccess.Repository.request;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinessObject.request
{
    public class RequestService : BaseService<Request>, IRequestService
    {
        private readonly IRequestRepository _requestRepository;

        public RequestService(IUnitOfWork unitOfWork, IRequestRepository requestRepository) : base(unitOfWork)
        {
            _requestRepository = requestRepository;
        }
       


    }
}
