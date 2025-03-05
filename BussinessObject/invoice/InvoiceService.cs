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

        // ✅ Lấy hóa đơn theo RequestID
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
        // Tính tổng doanh thu của tháng hiện tại (chỉ tính hóa đơn đã thanh toán)
        public async Task<decimal> CalculateTotalRevenueAsync()
        {
            try
            {
                var invoices = await _invoiceRepository.GetAllInvoices();
                var currentDate = DateTime.UtcNow;
                var currentMonth = currentDate.Month;
                var currentYear = currentDate.Year;

                var monthlyInvoices = invoices
                    .Where(i => i.CreatedAt.Month == currentMonth &&
                                i.CreatedAt.Year == currentYear &&
                                i.InvoiceStatus == InvoiceStatus.Paid)
                    .ToList();

                decimal totalRevenue = monthlyInvoices.Sum(i => i.TotalAmount);

                _logger.LogInformation("Successfully calculated Paid revenue for {Year}-{Month}: {TotalRevenue}",
                    currentYear, currentMonth, totalRevenue);
                return totalRevenue;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating Paid revenue for current month");
                throw;
            }
        }

        // Tính tổng doanh thu của năm hiện tại (chỉ tính hóa đơn đã thanh toán)
        public async Task<decimal> CalculateTotalRevenueForCurrentYearAsync()
        {
            try
            {
                var invoices = await _invoiceRepository.GetAllInvoices();
                var currentYear = DateTime.UtcNow.Year;

                var yearlyInvoices = invoices
                    .Where(i => i.CreatedAt.Year == currentYear &&
                                i.InvoiceStatus == InvoiceStatus.Paid)
                    .ToList();

                decimal totalRevenue = yearlyInvoices.Sum(i => i.TotalAmount);

                _logger.LogInformation("Successfully calculated Paid revenue for current year {Year}: {TotalRevenue}",
                    currentYear, totalRevenue);
                return totalRevenue;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating Paid revenue for current year");
                throw;
            }
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


        // Tính tổng doanh thu của ngày hôm nay (chỉ tính hóa đơn đã thanh toán)
        public async Task<decimal> CalculateTotalRevenueForTodayAsync()
        {
            try
            {
                var invoices = await _invoiceRepository.GetAllInvoices();
                var today = DateTime.UtcNow.Date;

                var todayInvoices = invoices
                    .Where(i => i.CreatedAt.Date == today &&
                                i.InvoiceStatus == InvoiceStatus.Paid)
                    .ToList();

                decimal totalRevenue = todayInvoices.Sum(i => i.TotalAmount);

                _logger.LogInformation("Successfully calculated Paid revenue for today {Today}: {TotalRevenue}",
                    today.ToString("yyyy-MM-dd"), totalRevenue);
                return totalRevenue;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating Paid revenue for today");
                throw;
            }
        }

        // Tính tổng doanh thu của ngày hôm qua (chỉ tính hóa đơn đã thanh toán)
        public async Task<decimal> CalculateTotalRevenueForYesterdayAsync()
        {
            try
            {
                var invoices = await _invoiceRepository.GetAllInvoices();
                var yesterday = DateTime.UtcNow.Date.AddDays(-1);

                var yesterdayInvoices = invoices
                    .Where(i => i.CreatedAt.Date == yesterday &&
                                i.InvoiceStatus == InvoiceStatus.Paid)
                    .ToList();

                decimal totalRevenue = yesterdayInvoices.Sum(i => i.TotalAmount);

                _logger.LogInformation("Successfully calculated Paid revenue for yesterday {Yesterday}: {TotalRevenue}",
                    yesterday.ToString("yyyy-MM-dd"), totalRevenue);
                return totalRevenue;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating Paid revenue for yesterday");
                throw;
            }
        }

        // Tính tổng doanh thu của tháng trước (chỉ tính hóa đơn đã thanh toán)
        public async Task<decimal> CalculateTotalRevenueForLastMonthAsync()
        {
            try
            {
                var invoices = await _invoiceRepository.GetAllInvoices();
                var currentDate = DateTime.UtcNow;
                var lastMonthDate = currentDate.AddMonths(-1);
                var lastMonth = lastMonthDate.Month;
                var lastMonthYear = lastMonthDate.Year;

                var lastMonthInvoices = invoices
                    .Where(i => i.CreatedAt.Month == lastMonth &&
                                i.CreatedAt.Year == lastMonthYear &&
                                i.InvoiceStatus == InvoiceStatus.Paid)
                    .ToList();

                decimal totalRevenue = lastMonthInvoices.Sum(i => i.TotalAmount);

                _logger.LogInformation("Successfully calculated Paid revenue for last month {Year}-{Month}: {TotalRevenue}",
                    lastMonthYear, lastMonth, totalRevenue);
                return totalRevenue;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating Paid revenue for last month");
                throw;
            }
        }

        // Tính tổng doanh thu của năm trước (chỉ tính hóa đơn đã thanh toán)
        public async Task<decimal> CalculateTotalRevenueForLastYearAsync()
        {
            try
            {
                var invoices = await _invoiceRepository.GetAllInvoices();
                var lastYear = DateTime.UtcNow.Year - 1;

                var lastYearInvoices = invoices
                    .Where(i => i.CreatedAt.Year == lastYear &&
                                i.InvoiceStatus == InvoiceStatus.Paid)
                    .ToList();

                decimal totalRevenue = lastYearInvoices.Sum(i => i.TotalAmount);

                _logger.LogInformation("Successfully calculated Paid revenue for last year {Year}: {TotalRevenue}",
                    lastYear, totalRevenue);
                return totalRevenue;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating Paid revenue for last year");
                throw;
            }
        }

        public async Task<List<Invoice>> GetAllInvoicesAsync()
        {
            try
            {
                var invoices = await _invoiceRepository.GetAllAsync();
                _logger.LogInformation("Successfully retrieved all invoices. Total count: {Count}", invoices.Count);
                return invoices.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all invoices");
                throw;
            }
        }

        public async Task<List<TopSellingItemDTO>> GetTopSellingItemsAsync(string timeRange, int topN = 5)
        {
            try
            {
                var invoices = await _invoiceRepository.GetAllInvoiceAsync();
                DateTime startDate;
                DateTime endDate = DateTime.UtcNow;

                // Xác định khoảng thời gian dựa trên timeRange
                switch (timeRange)
                {
                    case "Today":
                        startDate = DateTime.UtcNow.Date;
                        endDate = startDate.AddDays(1);
                        break;
                    case "This Month":
                        startDate = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
                        endDate = startDate.AddMonths(1);
                        break;
                    case "This Year":
                        startDate = new DateTime(DateTime.UtcNow.Year, 1, 1);
                        endDate = startDate.AddYears(1);
                        break;
                    default:
                        throw new ArgumentException("Invalid time range", nameof(timeRange));
                }

                // Lọc các hóa đơn trong khoảng thời gian và trạng thái Paid
                var filteredInvoices = invoices
                    .Where(i => i.CreatedAt >= startDate && i.CreatedAt < endDate && i.InvoiceStatus == InvoiceStatus.Paid)
                    .ToList();

                // Lấy tất cả chi tiết đơn hàng từ các hóa đơn đã lọc
                var orderDetails = filteredInvoices
                    .SelectMany(i => i.Request.OrderDetails)
                    .GroupBy(od => od.ItemId)
                    .Select(g => new
                    {
                        ItemId = g.Key,
                        QuantitySold = g.Sum(od => od.Quantity),
                        TotalRevenue = g.Sum(od => od.Quantity * od.Price),
                        Item = g.First().Item
                    })
                    .Where(x => x.Item != null) // Loại bỏ các bản ghi có Item là null
                    .OrderByDescending(x => x.TotalRevenue)
                    .Take(topN)
                    .ToList();

                // Chuyển đổi dữ liệu thành TopSellingItemDTO
                var topSellingItems = orderDetails
                    .Select(x => new TopSellingItemDTO
                    {
                        ItemName = x.Item.ItemName,
                        Price = x.Item.Price,
                        QuantitySold = x.QuantitySold,
                        TotalRevenue = x.TotalRevenue,
                        ImageUrl = x.Item.ImageUrl
                    })
                    .ToList();

                _logger.LogInformation("Successfully retrieved top {TopN} selling items for {TimeRange}.", topN, timeRange);
                return topSellingItems;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving top selling items for {TimeRange}", timeRange);
                throw;
            }
        }
    }
}
