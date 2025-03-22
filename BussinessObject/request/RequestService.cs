using BussinessObject;
using BussinessObject.DTOs;
using DataAccess.Enum;
using DataAccess.Models;
using DataAccess.Repository.Base;
using DataAccess.Repository.cancellation;
using DataAccess.Repository.invoice;
using DataAccess.Repository.orderdetail;
using DataAccess.Repository.request;
using DataAccess.Repository.servicecall;
using DataAccess.Repository.servicereason;
using MailKit.Search;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Security;
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
        private readonly IServiceCallRepository _serviceCallRepository;
        private readonly IServiceReasonRepository _serviceReasonRepository;
        private readonly ILogger<RequestService> _logger;

        public RequestService(IUnitOfWork unitOfWork,
            IRequestRepository requestRepository,
            IOrderDetailRepository orderDetailRepository,
            ICancellationReasonRepository cancellationReasonRepository,
            IInvoiceRepository invoiceRepository,
            IServiceCallRepository serviceCallRepository,
            IServiceReasonRepository serviceReasonRepository,
            ILogger<RequestService> logger) : base(unitOfWork)
        {
            _requestRepository = requestRepository;
            _orderDetailRepository = orderDetailRepository;
            _cancellationReasonRepository = cancellationReasonRepository;
            _invoiceRepository = invoiceRepository;
            _serviceCallRepository = serviceCallRepository;
            _serviceReasonRepository = serviceReasonRepository;
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

        //get request with note and return dtos
        public async Task<List<CustomerRequestDTO>> GetAllRequestsWithNotes()
        {
            var requests = await _requestRepository.GetPendingRequests();
            var requestDtos = new List<CustomerRequestDTO>();

            foreach (var request in requests)
            {
                var serviceCall = await _serviceCallRepository.GetServiceCallWithRequestId(request.RequestId);
                string note = "Không có ghi chú";

                //convert note sang tiếng việt
                //if (serviceCall != null && !string.IsNullOrEmpty(serviceCall.Note))
                //{
                //    string normalizedNote = serviceCall.Note.Trim(); //Loại bỏ khoảng trắng thừa

                //    _logger.LogInformation("🟢 ServiceCall Note Found: '{Note}' (Normalized: '{NormalizedNote}')", serviceCall.Note, normalizedNote);

                //    // 🟢 Nếu Note lưu dưới dạng Enum.ToString(), cần ánh xạ sang tiếng Việt
                //    if (Enum.TryParse(normalizedNote, out PaymentMethod method))
                //    {
                //        note = PaymentMethodEnumHelper.GetVietnameseName(method); // Hiển thị tiếng Việt
                //    }
                //    else
                //    {
                //        note = normalizedNote; // 🟢 Nếu không khớp, giữ nguyên giá trị
                //    }
                //}
                if (serviceCall != null && !string.IsNullOrEmpty(serviceCall.Note))
                {
                    note = serviceCall.Note.Trim();
                    _logger.LogInformation("🟢 ServiceCall Note Found: '{Note}'", note);
                }

                var customerRequestDTO = new CustomerRequestDTO
                {
                    RequestId = request.RequestId,
                    TableNumber = request.Table.TableNumber,
                    CustomerId = request.CustomerId ?? 0,
                    CustomerName = request.Customer.CustomerName,
                    RequestType = request.RequestType.RequestTypeName,
                    CreatedAt = request.CreatedAt ?? DateTime.UtcNow, // 🟢 Tránh lỗi null DateTime
                    Note = note
                };

                requestDtos.Add(customerRequestDTO);
            }

            return requestDtos;
        }

        public async Task<Request> GetCheckoutRequest(int customerId)
        {
            var request = await _requestRepository.GetCheckoutRequestByCustomer(customerId);
            return request;
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
            catch (Exception ex)
            {
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

            // 🟢 Kiểm tra nếu Request này là yêu cầu thanh toán (RequestTypeID = 3)
            if (request.RequestTypeId == 3)
            {
                // 🟢 Lấy ServiceCall của request (chứa phương thức thanh toán)
                var serviceCall = await _serviceCallRepository.GetServiceCallWithRequestId(requestId);
                if (serviceCall == null || string.IsNullOrEmpty(serviceCall.Note))
                {
                    return ServiceResult<Request>.CreateError("Payment method not found.");
                }

                var paymentMethod = Enum.Parse<PaymentMethod>(serviceCall.Note); // 🟢 Lấy phương thức thanh toán từ Note

                // 🟢 Lấy hóa đơn của khách hàng
                var invoice = await _invoiceRepository.GetInvoiceByCustomer(request.CustomerId.Value);
                if (invoice == null) return ServiceResult<Request>.CreateError("Invoice not found.");

                var updateSuccess = await _invoiceRepository.UpdatePaymentMethod(invoice.InvoiceId, paymentMethod);
                if (!updateSuccess) return ServiceResult<Request>.CreateError("Failed to update invoice payment method.");

                return ServiceResult<Request>.CreateSuccess(request, "Payment request accepted, invoice updated.");
            }

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

        public async Task<int> AddRequestOrder(List<OrderItemDto> orderItems, OrderByDto orderBy)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var newRequest = new Request
                {
                    TableId = orderBy.TableId,
                    CustomerId = orderBy.CustomerId,
                    RequestTypeId = 1,
                    RequestStatusId = 1,
                    CreatedAt = DateTime.Now,
                };
                await _requestRepository.AddAsync(newRequest);
                await _unitOfWork.SaveChangesAsync();

                var requesetId = newRequest.RequestId;
                foreach (var orderItem in orderItems)
                {
                    var newOrderDetail = new OrderDetail
                    {
                        RequestId = requesetId,
                        ItemId = orderItem.Id,
                        Quantity = orderItem.Quantity,
                        Price = orderItem.Price * orderItem.Quantity,
                        Note = "N/A"
                    };

                    await _orderDetailRepository.AddAsync(newOrderDetail);
                }

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                return 1;
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task<Request> GetPendingFoodOrderRequest(int customerId)
        {
            return await _requestRepository.GetServingFoodOrderRequest(customerId);
        }

        public async Task<ServiceResult<Request>> CreatePaymentRequest(PaymentRequestDTO requestDto)
        {
            try
            {
                // 🟢 Kiểm tra khách hàng có hóa đơn chưa thanh toán không
                var invoice = await _invoiceRepository.GetInvoiceByCustomer(requestDto.CustomerId);
                if (invoice == null || invoice.InvoiceStatus != InvoiceStatus.Serving)
                {
                    return ServiceResult<Request>.CreateError("No active invoice found for payment.");
                }

                // Tạo Request thanh toán
                var paymentRequest = new Request
                {
                    TableId = requestDto.TableId,
                    CustomerId = requestDto.CustomerId,
                    RequestTypeId = 3, // Thanh toán
                    RequestStatusId = 1, // Pending
                    CreatedAt = DateTime.UtcNow
                };

                var createdRequest = await _requestRepository.AddNewRequest(paymentRequest);
                if (createdRequest == null)
                {
                    return ServiceResult<Request>.CreateError("Failed to create payment request.");
                }

                // Tạo ServiceCall với lý do thanh toán (ReasonID = 3) và lưu PaymentMethod vào Note
                var reasonId = await _serviceReasonRepository.GetReasonDefaultId();
                var serviceCall = new ServiceCall
                {
                    RequestId = createdRequest.RequestId,
                    ReasonId = reasonId, // Lý do: Thanh toán-default
                    Note = requestDto.PaymentMethod.ToString()
                };

                var serviceCallSuccess = await _serviceCallRepository.AddServiceCall(serviceCall);
                if (!serviceCallSuccess)
                {
                    return ServiceResult<Request>.CreateError("Failed to create service call for payment request.");
                }

                return ServiceResult<Request>.CreateSuccess(createdRequest, "Payment request created successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error processing payment request.");
                return ServiceResult<Request>.CreateError("An error occurred while processing payment request.");
            }
        }

        public async Task<int> AddRequestService(ServiceCallResponseDto dto)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var newRequest = new Request
                {
                    TableId = dto.tableId,
                    CustomerId = dto.customerId,
                    RequestTypeId = 2,
                    RequestStatusId = 1,
                    CreatedAt = DateTime.Now,
                };
                await _requestRepository.AddAsync(newRequest);
                await _unitOfWork.SaveChangesAsync();

                var requesetId = newRequest.RequestId;
                StringBuilder note = new StringBuilder();
                foreach (var Service in dto.ListReson)
                {
                    note.Append(Service.ToString());
                    note.Append(", ");
                }
                if(dto.CustomService.IsNullOrEmpty())
                {
                     note.Remove(note.Length - 2, 2);
                }
                else
                {
                note.Append(dto.CustomService.ToString());
                }
                var ReasonId = await _serviceReasonRepository.GetReasonDefaultId();
                
                var ServiceCall = new ServiceCall
                {
                    RequestId = requesetId,
                    ReasonId = ReasonId,
                    Note = note.ToString(),
                };
                await _serviceCallRepository.AddAsync(ServiceCall);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                return 1;
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task<ServiceResult<Request>> CreateVnPayRequestAsync(int customerId, int tableId)
        {
            try
            {
                var newRequest = await _requestRepository.createVnpayRequest(customerId, tableId);
                return ServiceResult<Request>.CreateSuccess(newRequest);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo request thanh toán.");
                return ServiceResult<Request>.CreateError("Không thể tạo yêu cầu thanh toán.");
            }
        }



        public async Task<List<Request>> GetAllRequestsAsync()
        {
            try
            {
                // Chỉ lấy các yêu cầu có trạng thái Accepted (3) hoặc Rejected (4)
                var requests = await _requestRepository.GetAllAsync();
                requests = requests.Where(r => r.RequestStatusId == 3 || r.RequestStatusId == 4).ToList();

                foreach (var request in requests)
                {
                    await _requestRepository.LoadRequestRelations(request); // Load các quan hệ liên quan
                }
                return requests;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all requests");
                return new List<Request>();
            }
        }
    }
}
