using DataAccess.Models;
using DataAccess.Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository.request
{
    public interface IRequestRepository : IBaseRepository<Request>
    {
        Task<List<Request>> GetPendingRequests(string type = "All");
        Task<Request> GetRequestById(int requestId);
        Task<Request> GetPendingFoodOrderRequest(int customerId);
        Task<bool> UpdateRequestStatus(int requestId, int newStatusId, int? accountId = null);
        Task<bool> RejectRequest(int requestId, int reasonId, int? accountId = null);

        Task LoadRequestRelations(Request request);

        Task<List<Request>> GetCustomerInProcessRequests(int customerId, int? accountId = null);

        Task<bool> MarkRequestInProcess(int requestId, int? accountId = null);
        Task<bool> ResetPendingRequest(int requestId);

        Task<Request> GetLatestAcceptedRequest(int customerId);

        Task<List<Request>> GetAcceptedOrdersAsync(string filter = "Accepted");

        Task<bool> SaveChanges();
    }
}
