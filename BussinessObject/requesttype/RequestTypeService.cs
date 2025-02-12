using BussinessObject;
using DataAccess.Models;
using DataAccess.Repository.Base;
using DataAccess.Repository.requesttype;
using System;
using System.Threading.Tasks;

namespace BussinessObject.requesttype
{
    public class RequestTypeService : BaseService<RequestType>, IRequestTypeService
    {
        private readonly IRequestTypeRepository _requestTypeRepository;

        public RequestTypeService(IUnitOfWork unitOfWork, IRequestTypeRepository requestTypeRepository) : base(unitOfWork)
        {
            _requestTypeRepository = requestTypeRepository;
        }

       
    }
}
