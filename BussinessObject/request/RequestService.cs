using BussinessObject;
using BussinessObject.Dto;
using DataAccess.Models;
using DataAccess.Repository.Base;
using DataAccess.Repository.orderdetail;
using DataAccess.Repository.request;

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
        private readonly IOrderDetailRepository _orderDetailRepository;

        public RequestService(IUnitOfWork unitOfWork, IRequestRepository requestRepository, IOrderDetailRepository orderDetailRepository) : base(unitOfWork)
        {
            _requestRepository = requestRepository;
            _orderDetailRepository = orderDetailRepository;
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
                foreach(var orderItem in orderItems)
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
            return await _requestRepository.GetPendingFoodOrderRequest(customerId);
        }
    }
}
