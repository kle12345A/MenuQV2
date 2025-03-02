using BussinessObject;
using BussinessObject.Dto;
using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinessObject.request
{
    public interface IRequestService : IBaseService<Request>
    {
        Task<int> AddRequestOrder(List<OrderItemDto> orderItems, OrderByDto orderBy);
        Task<Request> GetPendingFoodOrderRequest(int customerId);
        Task<List<Request>> GetPendingRequests(string type = "All");
        Task<Request> GetRequestDetailsAsync(int requestId);
        Task<List<CancellationReason>> GetActiveCancellationReasons();
        Task<ServiceResult<Request>> ProcessRequest(int requestId, int? accountId = null);
        Task<ServiceResult<Request>> ResetPendingRequest(int requestId);

        Task<ServiceResult<Request>> RejectRequest(int requestId, int reasonId, int? accountId = null);
        Task<ServiceResult<Request>> AcceptRequest(int requestId, int? accountId = null);

        Task<ServiceResult<Request>> ProcessFoodOrder(int customerId, List<OrderDetail> orderDetails);

        Task<List<Request>> GetAcceptedOrdersAsync(string filter = "Accepted");

        Task<List<Request>> GetAllRequestsAsync();
    }
}
