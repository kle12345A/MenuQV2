using BussinessObject;
using DataAccess.Models;
using DataAccess.Repository.Base;
using DataAccess.Repository.servicecall;

using System;
using System.Threading.Tasks;

namespace BussinessObject.servicecall
{
    public class ServiceCallService : BaseService<ServiceCall>, IServiceCallService
    {
        private readonly IServiceCallRepository _serviceCallRepository;

        public ServiceCallService(IUnitOfWork unitOfWork, IServiceCallRepository serviceCallRepository) : base(unitOfWork)
        {
            _serviceCallRepository = serviceCallRepository;
        }

    }
}
