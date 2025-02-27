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

    }
}
