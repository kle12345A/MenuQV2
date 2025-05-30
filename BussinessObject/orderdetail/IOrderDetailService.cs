﻿using BussinessObject;
using DataAccess.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinessObject.orderdetail
{
    public interface IOrderDetailService : IBaseService<OrderDetail>
    {
        Task<List<OrderDetail>> GetOrderDetailsByRequestId(int requestId);
        Task<ServiceResult<bool>> UpdateOrderItemQuantity(int orderDetailId, int newQuantity);
        Task UpdateInvoiceTotal(int requestId);
       // Task<IEnumerable<OrderDetail>> GetAllOrderDetailsAsync();

    }
}
