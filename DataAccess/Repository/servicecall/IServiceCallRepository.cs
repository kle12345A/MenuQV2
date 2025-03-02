using DataAccess.Models;
using DataAccess.Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository.servicecall
{
    public interface IServiceCallRepository : IBaseRepository<ServiceCall>
    {
        Task<bool> AddServiceCall(ServiceCall serviceCall);
        Task<ServiceCall?> GetServiceCallWithRequestId(int requestId);
        Task<bool> SaveChanges();
    }
}
