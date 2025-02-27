using BussinessObject.invoice;
using DataAccess.Enum;
using DataAccess.Models;
using DataAccess.Repository.Base;
using DataAccess.Repository.cancellation;
using DataAccess.Repository.invoice;
using DataAccess.Repository.orderdetail;
using DataAccess.Repository.request;
using Microsoft.Extensions.Logging;
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
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IOrderDetailRepository _orderDetailRepository;
        private readonly ICancellationReasonRepository _cancellationReasonRepository;
        private readonly ILogger<RequestService> _logger;

        public RequestService(IUnitOfWork unitOfWork, 
            IRequestRepository requestRepository,
            IOrderDetailRepository orderDetailRepository,
            ICancellationReasonRepository cancellationReasonRepository, 
            IInvoiceRepository invoiceRepository,
            ILogger<RequestService> logger) : base(unitOfWork)
        {
            _requestRepository = requestRepository;
            _orderDetailRepository = orderDetailRepository;
            _cancellationReasonRepository = cancellationReasonRepository;
            _invoiceRepository = invoiceRepository;
            _logger = logger;
        }  

        public async Task<List<CancellationReason>> GetActiveCancellationReasons()
        {
            try
            {
                return await _cancellationReasonRepository.GetActiveCancellationReasons();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting cancellation reasons");
                return new List<CancellationReason>();
            }
        }

        //get pending request with filter
        public async Task<List<Request>> GetPendingRequests(string type = "All")
        {
            try
            {
                var requests = await _requestRepository.GetPendingRequests(type);
                // Không cần LoadRequestRelations nữa vì đã Include trong query
                return requests;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting pending requests");
                return new List<Request>();
            }
        }

        public async Task<Request> GetRequestDetailsAsync(int requestId)
        {
            try
            {
                var request = await _requestRepository.GetByIdAsync(requestId);

                if (request == null) return null;

                //Load đầy đủ các quan hệ liên quan
                await _requestRepository.LoadRequestRelations(request);

                return request;
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Error getting pending requests");
                return new Request();
            }
        }

        //method to process request 
        //change requeststatusId = 2 (in process) meaning an employee is viewing the request
        public async Task<ServiceResult<Request>> ProcessRequest(int requestId, int? accountId = null)
        {
            try
            {
                var request = await _requestRepository.GetRequestById(requestId);
                if (request == null)
                    return ServiceResult<Request>.CreateError("Request not found");

                if (request.RequestStatusId == 2) // Nếu đã là InProcess thì không cập nhật lại
                    return ServiceResult<Request>.CreateSuccess(request, "Request is already in process");

                if (request.RequestStatusId != 1) // Nếu không phải Pending thì không xử lý
                    return ServiceResult<Request>.CreateError("Request has already been processed");

                var success = await _requestRepository.MarkRequestInProcess(requestId, accountId);
                return success
                    ? ServiceResult<Request>.CreateSuccess(request, "Request is now being processed")
                    : ServiceResult<Request>.CreateError("Failed to update request status");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing request {RequestId}", requestId);
                return ServiceResult<Request>.CreateError("An error occurred while processing the request");
            }
        }

        //trả trạng thái của đơn về 1 nếu không thao tác gì cả 
        public async Task<ServiceResult<Request>> ResetPendingRequest(int requestId)
        {
            try
            {
                _logger.LogInformation("Service: Resetting request status for RequestID: {RequestId}", requestId);

                var request = await _requestRepository.GetRequestById(requestId);
                if (request == null)
                {
                    _logger.LogWarning("Service: Request ID {RequestId} not found in database.", requestId);
                    return ServiceResult<Request>.CreateError("Request not found");
                }

                if (request.RequestStatusId != 2) // Chỉ reset nếu đang InProcess
                {
                    _logger.LogWarning("Service: Request ID {RequestId} is not in 'InProcess' state.", requestId);
                    return ServiceResult<Request>.CreateError("Request is not currently being processed");
                }

                var success = await _requestRepository.ResetPendingRequest(requestId);
                return success
                    ? ServiceResult<Request>.CreateSuccess(request, "Request status reset to Pending")
                    : ServiceResult<Request>.CreateError("Failed to reset request status");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Service: Error resetting request status for RequestID: {RequestId}", requestId);
                return ServiceResult<Request>.CreateError("An error occurred while resetting request status");
            }
        }

        //method to accpet request 
        //change requestStatusId = 3 (completed)
        public async Task<ServiceResult<Request>> AcceptRequest(int requestId, int? accountId = null)
        {
            var request = await _requestRepository.GetRequestById(requestId);
            if (request == null) return ServiceResult<Request>.CreateError("Request not found");
            if (request.RequestStatusId != 1 && request.RequestStatusId != 2)
                return ServiceResult<Request>.CreateError("Request is not in a pending or in-process state");

            var success = await _requestRepository.UpdateRequestStatus(requestId, 3, accountId);
            if (!success) return ServiceResult<Request>.CreateError("Failed to update request status");

            //Kiểm tra nếu khách hàng đã có hóa đơn "Serving"
            var existingInvoice = await _invoiceRepository.GetInvoiceByCustomer(request.CustomerId.Value);

            if (existingInvoice != null && existingInvoice.InvoiceStatus == InvoiceStatus.Serving)
            {
                //Nếu đã có Invoice "Serving", chỉ cập nhật OrderDetails
                await _invoiceRepository.UpdateInvoiceWithNewOrderDetails(existingInvoice.InvoiceId, (List<OrderDetail>)request.OrderDetails);

                //Cập nhật lại tổng tiền hóa đơn
                var updatedTotal = (await _orderDetailRepository.GetOrderDetailsByCustomerId(request.CustomerId.Value))
                    .Sum(od => od.Quantity * od.Price);

                await _invoiceRepository.UpdateInvoiceTotal(existingInvoice.InvoiceId, updatedTotal);
            }
            else
            {
                // 🔴 Nếu chưa có hóa đơn, tạo hóa đơn mới
                var newInvoice = new Invoice
                {
                    RequestId = requestId,
                    CustomerId = request.CustomerId.Value,
                    InvoiceCode = $"INV{DateTime.UtcNow.Ticks}",
                    TotalAmount = request.OrderDetails?.Sum(od => od.Quantity * od.Price) ?? 0,
                    InvoiceStatus = InvoiceStatus.Serving,
                    PaymentStatus = false,
                    PaymentMethod = "Unknown" // Tránh lỗi NULL
                };

                var createdInvoice = await _invoiceRepository.CreateInvoice(newInvoice);
                if (!createdInvoice)
                {
                    _logger.LogError("Failed to create invoice for Request {RequestId}", requestId);
                    return ServiceResult<Request>.CreateError("Failed to create new invoice.");
                }
            }

            return ServiceResult<Request>.CreateSuccess(request, "Request accepted successfully.");
        }




        private async Task<bool> UpdateInvoiceForRequest(int requestId)
        {
            var invoice = await _invoiceRepository.GetInvoiceByRequestId(requestId);
            if (invoice == null) return false;

            var updatedTotal = (await _orderDetailRepository.GetOrderDetailsByRequestId(requestId))
                .Sum(od => od.Quantity * od.Price);

            return await _invoiceRepository.UpdateInvoiceTotal(invoice.InvoiceId, updatedTotal);
        }



        //Method to reject request
        //change requestStatusId = 4 (canacelled) 
        public async Task<ServiceResult<Request>> RejectRequest(int requestId, int reasonId, int? accountId = null)
        {
            var request = await _requestRepository.GetRequestById(requestId);
            if (request == null) return ServiceResult<Request>.CreateError("Request not found");
            if (request.RequestStatusId != 1 && request.RequestStatusId != 2)
                return ServiceResult<Request>.CreateError("Request cannot be rejected in its current state");

            _logger.LogInformation("Rejecting RequestID: {RequestId} with ReasonID: {ReasonId}", requestId, reasonId);

            var success = await _requestRepository.UpdateRequestStatus(requestId, 4, accountId, reasonId);
            if (!success) return ServiceResult<Request>.CreateError("Failed to update request status");

            var invoice = await _invoiceRepository.GetInvoiceByRequestId(requestId);
            if (invoice != null && invoice.InvoiceStatus == InvoiceStatus.Serving)
            {
                var invoiceSuccess = await _invoiceRepository.UpdateInvoiceStatus(invoice.InvoiceId, InvoiceStatus.Cancelled);
                if (!invoiceSuccess) return ServiceResult<Request>.CreateError("Failed to update invoice status");
            }

            return ServiceResult<Request>.CreateSuccess(request, "Request rejected successfully");
        }



        public async Task<ServiceResult<Request>> ProcessFoodOrder(int customerId, List<OrderDetail> orderDetails)
        {
            var existingInvoice = await _invoiceRepository.GetInvoiceByCustomer(customerId);

            var newRequest = new Request
            {
                CustomerId = customerId,
                RequestTypeId = 1,
                RequestStatusId = 1,
                OrderDetails = orderDetails,
                CreatedAt = DateTime.UtcNow
            };
            await _requestRepository.AddAsync(newRequest);
            var saveSuccess = await _requestRepository.SaveChanges();
            if (!saveSuccess) return ServiceResult<Request>.CreateError("Failed to save new request");

            if (existingInvoice != null)
            {
                await _invoiceRepository.UpdateInvoiceWithNewOrderDetails(existingInvoice.InvoiceId, orderDetails);
                return ServiceResult<Request>.CreateSuccess(newRequest, "Order updated successfully, pending approval.");
            }

            var invoiceCreated = await _invoiceRepository.CreateInvoice(new Invoice
            {
                RequestId = newRequest.RequestId,
                CustomerId = customerId,
                InvoiceCode = "INV" + DateTime.UtcNow.Ticks,
                TotalAmount = orderDetails.Sum(od => od.Quantity * od.Price),
                InvoiceStatus = InvoiceStatus.Serving,
                PaymentStatus = false
            });

            return invoiceCreated
                ? ServiceResult<Request>.CreateSuccess(newRequest, "New request pending approval.")
                : ServiceResult<Request>.CreateError("Failed to create invoice");
        }



        public async Task<List<Request>> GetAcceptedOrdersAsync(string filter = "Accepted")
        {
            return await _requestRepository.GetAcceptedOrdersAsync(filter);
        }


    }
}
