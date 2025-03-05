using BussinessObject;
using BussinessObject.DTOs;
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
        Task<ServiceResult<Request>> CreatePaymentRequest(PaymentRequestDTO requestDto);
        Task<Request> GetPendingFoodOrderRequest(int customerId);
        Task<List<Request>> GetPendingRequests(string type = "All");
        Task<Request> GetRequestDetailsAsync(int requestId);
        Task<Request> GetCheckoutRequest(int customerId);   
        Task<List<CancellationReason>> GetActiveCancellationReasons();
        //get request with note
        Task<List<CustomerRequestDTO>> GetAllRequestsWithNotes();

        Task<ServiceResult<Request>> ProcessRequest(int requestId, int? accountId = null);
        Task<ServiceResult<Request>> ResetPendingRequest(int requestId);

        Task<ServiceResult<Request>> RejectRequest(int requestId, int reasonId, int? accountId = null);
        Task<ServiceResult<Request>> AcceptRequest(int requestId, int? accountId = null);

        Task<ServiceResult<Request>> ProcessFoodOrder(int customerId, List<OrderDetail> orderDetails);

        Task<List<Request>> GetAcceptedOrdersAsync(string filter = "Accepted");


    }
}
