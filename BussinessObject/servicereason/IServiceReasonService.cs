using BussinessObject;
using DataAccess.Models;
using System;
using System.Threading.Tasks;

namespace BussinessObject.servicereason
{
    public interface IServiceReasonService : IBaseService<ServiceReason>
    {
        Task<List<ServiceReason>> GetAllActive();
    }
}
