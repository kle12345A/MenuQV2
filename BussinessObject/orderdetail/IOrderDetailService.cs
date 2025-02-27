using BussinessObject;
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
        Task<IEnumerable<OrderDetail>> GetAllOrderDetailsAsync();

    }
}
