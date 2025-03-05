using BussinessObject.orderdetail;
using BussinessObject.DTOs;
using DataAccess.Enum;
using DataAccess.Models;
using DataAccess.Repository.Base;
using DataAccess.Repository.invoice;
using DataAccess.Repository.orderdetail;
using DataAccess.Repository.request;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BussinessObject.invoice
{
    public class InvoiceService : BaseService<Invoice>, IInvoiceService
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IRequestRepository _requestRepository;
        private readonly IOrderDetailRepository _orderDetailRepository;
        private readonly ILogger<InvoiceService> _logger;

        public InvoiceService(IUnitOfWork unitOfWork,
                                IInvoiceRepository invoiceRepository,
                                IRequestRepository requestRepository,
                                IOrderDetailRepository orderDetailRepository,
                                ILogger<InvoiceService> logger) : base(unitOfWork)
        {
            _invoiceRepository = invoiceRepository;
            _requestRepository = requestRepository;
            _orderDetailRepository = orderDetailRepository;
            _logger = logger;
        }

        public async Task<List<InvoiceDTO>> GetAllAsync()
        {
            var invoices = await _invoiceRepository.GetAllInvoices();
            return invoices.Select(i => new InvoiceDTO
            {
                RequestId = i.RequestId,
                InvoiceCode = i.InvoiceCode,
                TableId = i.TableId, 
                TableName = i.Table.TableNumber,
                CustomerId = i.CustomerId,
                CustomerName = i.Customer.CustomerName,
                PhoneNumber = i.Customer.PhoneNumber,
                TotalAmount = i.TotalAmount,
                InvoiceStatus = i.InvoiceStatus.ToString()
            }).ToList();
        }

        //Lấy hóa đơn theo RequestID
        public async Task<InvoiceDetailDTO> GetInvoiceByRequestId(int requestId)
        {
            var invoice = await _invoiceRepository.GetInvoiceByRequestId(requestId);
            if (invoice == null) return null;

            return new InvoiceDetailDTO
            {
                InvoiceId = invoice.InvoiceId,
                InvoiceCode = invoice.InvoiceCode,
                CreatedAt = invoice.CreatedAt,
                CustomerName = invoice.Customer.CustomerName,
                CustomerId = invoice.CustomerId,
                PhoneNumber = invoice.Customer.PhoneNumber,
                TableId = invoice.TableId, 
                TableName = invoice.Table.TableNumber, 
                TotalAmount = invoice.TotalAmount,
                PaymentMethod = invoice.PaymentMethod,
                InvoiceStatus = invoice.InvoiceStatus.ToString(),
                OrderDetails = invoice.Request.OrderDetails.Select(od => new OrderDetailDTO
                {
                    ItemName = od.Item.ItemName,
                    Quantity = od.Quantity,
                    TotalPrice = od.Price * od.Quantity
                }).ToList()
            };
        }

        //Lấy hóa đơn theo CustomerID nếu đang có hóa đơn Serving
        public async Task<InvoiceDetailDTO> GetInvoiceByCustomer(int customerId)
        {
            try
            {
                var invoice = await _invoiceRepository.GetInvoiceByCustomer(customerId);
                if (invoice == null) return null;

                return new InvoiceDetailDTO
                {
                    InvoiceId = invoice.InvoiceId,
                    InvoiceCode = invoice.InvoiceCode,
                    CustomerName = invoice.Customer?.CustomerName ?? "Không có dữ liệu",
                    CustomerId = invoice.CustomerId,
                    PhoneNumber = invoice.Customer?.PhoneNumber ?? "N/A",
                    TableName = invoice.Table.TableNumber,
                    TotalAmount = invoice.TotalAmount,
                    PaymentMethod = invoice.PaymentMethod ?? "Unknown",
                    InvoiceStatus = ((int)invoice.InvoiceStatus).ToString(),
                    OrderDetails = invoice.Request?.OrderDetails?.Select(od => new OrderDetailDTO
                    {
                        ItemName = od.Item?.ItemName ?? "Không xác định",
                        Quantity = od.Quantity,
                        TotalPrice = od.Price * od.Quantity
                    }).ToList() ?? new List<OrderDetailDTO>() //Tránh lỗi null
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "rror retrieving invoice for CustomerID: {CustomerId}", customerId);
                return null;
            }
        }

        // Tạo hóa đơn khi khách đặt món
        public async Task<ServiceResult<Invoice>> CreateInvoiceForRequest(int requestId)
        {
            try
            {
                var request = await _requestRepository.GetRequestById(requestId);
                if (request == null)
                {
                    _logger.LogWarning("Request not found for ID: {RequestId}", requestId);
                    return ServiceResult<Invoice>.CreateError("Request not found");
                }

                var existingInvoice = await _invoiceRepository.GetInvoiceByRequestId(requestId);
                if (existingInvoice != null)
                {
                    _logger.LogInformation("Invoice already exists for Request ID: {RequestId}", requestId);
                    return ServiceResult<Invoice>.CreateSuccess(existingInvoice, "Invoice already exists.");
                }

                var invoice = new Invoice
                {
                    RequestId = requestId,
                    CustomerId = (int)request.CustomerId,
                    TableId = request.TableId ?? 0, // 🟢 Đảm bảo TableId không null
                    InvoiceCode = "INV" + DateTime.UtcNow.Ticks,
                    TotalAmount = request.OrderDetails.Sum(od => od.Quantity * od.Price),
                    InvoiceStatus = InvoiceStatus.Serving,
                    PaymentStatus = false
                };

                var success = await _invoiceRepository.CreateInvoice(invoice);
                return success
                    ? ServiceResult<Invoice>.CreateSuccess(invoice, "Invoice created successfully.")
                    : ServiceResult<Invoice>.CreateError("Failed to create invoice.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating invoice for RequestID {RequestId}", requestId);
                return ServiceResult<Invoice>.CreateError("An error occurred while creating invoice.");
            }
        }

        //Cập nhật hóa đơn khi có thay đổi
        public async Task<ServiceResult<Invoice>> UpdateInvoiceForRequest(int requestId)
        {
            try
            {
                var invoice = await _invoiceRepository.GetInvoiceByRequestId(requestId);
                if (invoice == null)
                {
                    _logger.LogWarning("Invoice not found for Request ID: {RequestId}", requestId);
                    return ServiceResult<Invoice>.CreateError("Invoice not found");
                }

                // 🟢 Đảm bảo không làm mất TableID khi cập nhật
                var request = await _requestRepository.GetRequestById(requestId);
                if (request != null && request.TableId.HasValue)
                {
                    invoice.TableId = request.TableId.Value;
                }

                var updatedTotal = (await _orderDetailRepository.GetOrderDetailsByRequestId(requestId))
                    .Sum(od => od.Quantity * od.Price);

                var success = await _invoiceRepository.UpdateInvoiceTotal(invoice.InvoiceId, updatedTotal);

                return success
                    ? ServiceResult<Invoice>.CreateSuccess(invoice, "Invoice updated successfully.")
                    : ServiceResult<Invoice>.CreateError("Failed to update invoice.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error updating invoice for Request ID: {RequestId}", requestId);
                return ServiceResult<Invoice>.CreateError("An error occurred while updating invoice.");
            }
        }




        //Cập nhật trạng thái hóa đơn
        public async Task<ServiceResult<Invoice>> UpdateInvoiceStatus(int invoiceId, InvoiceStatus status)
        {
            try
            {
                var invoice = await _invoiceRepository.GetByIdAsync(invoiceId);
                if (invoice == null)
                {
                    _logger.LogWarning("Invoice not found for ID: {InvoiceId}", invoiceId);
                    return ServiceResult<Invoice>.CreateError("Invoice not found");
                }

                invoice.InvoiceStatus = status; // Lưu kiểu Enum dưới dạng int
                var success = await _invoiceRepository.UpdateInvoice(invoice);

                return success
                    ? ServiceResult<Invoice>.CreateSuccess(invoice, $"Invoice status updated to {status}.")
                    : ServiceResult<Invoice>.CreateError("Failed to update invoice status.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating invoice status for ID: {InvoiceId}", invoiceId);
                return ServiceResult<Invoice>.CreateError("An error occurred while updating invoice status.");
            }
        }


        //Xử lý thanh toán
        public async Task<ServiceResult<Invoice>> Checkout(int invoiceId)
        {
            try
            {
                var invoice = await _invoiceRepository.GetByIdAsync(invoiceId);
                if (invoice == null)
                {
                    _logger.LogWarning("Invoice not found for invoiceId: {invoiceId}", invoiceId);
                    return ServiceResult<Invoice>.CreateError("Invoice not found");
                }

                //invoice.PaymentMethod = paymentMethod;
                invoice.PaymentStatus = true;
                invoice.PaymentDate = DateTime.UtcNow;
                invoice.InvoiceStatus = InvoiceStatus.Paid;

               // var success = await _invoiceRepository.UpdateInvoice(invoice.InvoiceId, InvoiceStatus.Paid);
                var success = await _invoiceRepository.UpdateInvoice(invoice);
                return success
                    ? ServiceResult<Invoice>.CreateSuccess(invoice, "Payment successful.")
                    : ServiceResult<Invoice>.CreateError("Payment failed.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing payment for invoiceId: {invoiceId}", invoiceId);
                return ServiceResult<Invoice>.CreateError("An error occurred while processing payment.");
            }
        }

        public async Task<bool> UpdateInvoiceWithNewOrderDetails(int invoiceId, List<OrderDetail> newOrderDetails)
        {
            var invoice = await _invoiceRepository.GetInvoiceByRequestId(invoiceId);
            if (invoice == null) return false;

            // Thêm từng OrderDetail vào danh sách OrderDetails của Request
            foreach (var newDetail in newOrderDetails)
            {
                invoice.Request.OrderDetails.Add(newDetail);
            }

            // Cập nhật tổng tiền Invoice
            var totalAmount = invoice.Request.OrderDetails.Sum(od => od.Quantity * od.Price);
            await _invoiceRepository.UpdateInvoiceTotal(invoice.InvoiceId, totalAmount);

            return true;
        }

        public async Task<bool> UpdatePaymentMethod(int invoiceId, string paymentMethod)
        {
            var invoice = await _invoiceRepository.GetByIdAsync(invoiceId);
            if (invoice == null) return false;

            invoice.PaymentMethod = paymentMethod ?? "Unknown"; 
            return await _invoiceRepository.UpdateInvoice(invoice);
        }



        public async Task<bool> ResetPaymentMethod(int invoiceId)
        {
            return await UpdatePaymentMethod(invoiceId, "Unknown");
        }


    }
}
