using BussinessObject;
using DataAccess.Models;
using DataAccess.Repository.Base;
using DataAccess.Repository.invoice;
using DataAccess.Repository.orderdetail;
using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinessObject.orderdetail
{
    public class OrderDetailService : BaseService<OrderDetail>, IOrderDetailService
    {
        private readonly IOrderDetailRepository _orderDetailRepository;
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly ILogger<OrderDetailService> _logger;
        public OrderDetailService(IUnitOfWork unitOfWork, IOrderDetailRepository orderDetailRepository, IInvoiceRepository invoiceRepository, ILogger<OrderDetailService> logger) : base(unitOfWork)
        {
            _orderDetailRepository = orderDetailRepository;
            _invoiceRepository = invoiceRepository;
            _logger = logger;
        }
        public async Task<List<OrderDetail>> GetOrderDetailsByRequestId(int requestId)
        {
            try
            {
                var orderDetails = await _orderDetailRepository.GetOrderDetailsByRequestId(requestId);

                if (orderDetails == null || orderDetails.Count == 0)
                {
                    _logger.LogWarning("No order details found for RequestID {RequestId}", requestId);
                    return new List<OrderDetail>();
                }

                return orderDetails;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching order details for RequestID {RequestId}", requestId);
                return new List<OrderDetail>();
            }
        }


        public async Task<ServiceResult<bool>> UpdateOrderItemQuantity(int orderDetailId, int newQuantity)
        {
            try
            {
                var orderDetail = await _orderDetailRepository.GetByIdAsync(orderDetailId);
                if (orderDetail == null)
                {
                    _logger.LogWarning("OrderDetail ID {OrderDetailId} not found.", orderDetailId);
                    return ServiceResult<bool>.CreateError("Order detail not found.");
                }

                if (newQuantity > 0)
                {
                    orderDetail.Quantity = newQuantity;
                    await _orderDetailRepository.UpdateAsync(orderDetail);
                }
                else
                {
                    await _orderDetailRepository.DeleteAsync(orderDetail);
                }
                await _invoiceRepository.UpdateInvoiceTotal((int)orderDetail.RequestId, 
                    await _orderDetailRepository.GetOrderDetailsByRequestId((int)orderDetail.RequestId).ContinueWith(t => t.Result.Sum(od => od.Quantity * od.Price)));
                return ServiceResult<bool>.CreateSuccess(true, "Order item updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating order item with ID {OrderDetailId}", orderDetailId);
                return ServiceResult<bool>.CreateError("An error occurred while updating the order item.");
            }
        }

        public async Task UpdateInvoiceTotal(int requestId)
        {
            try
            {
                var invoice = await _invoiceRepository.GetInvoiceByRequestId(requestId);
                if (invoice != null)
                {
                    //Lấy danh sách OrderDetail của Request và tính tổng tiền
                    var updatedTotal = (await _orderDetailRepository.GetOrderDetailsByRequestId(requestId))
                        .Sum(od => od.Quantity * od.Price);

                    //Cập nhật tổng tiền trong Invoice
                    var success = await _invoiceRepository.UpdateInvoiceTotal(invoice.InvoiceId, updatedTotal);

                    if (success)
                    {
                        _logger.LogInformation("💰 Invoice {InvoiceId} updated. New Total: {UpdatedTotal}", invoice.InvoiceId, updatedTotal);
                    }
                    else
                    {
                        _logger.LogWarning("⚠ Failed to update Invoice {InvoiceId}", invoice.InvoiceId);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error updating invoice total for Request ID {RequestId}", requestId);
            }
        }


    }
}

