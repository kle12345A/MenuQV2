using DataAccess.Models;
using DataAccess.Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository.orderdetail
{
    public interface IOrderDetailRepository : IBaseRepository<OrderDetail>
    {
        Task<List<OrderDetail>> GetOrderDetailsByRequestId(int requestId);
        Task<List<OrderDetail>> GetOrderDetailsByCustomerId(int customerId);
        Task<bool> UpdateOrderDetail(int orderDetailId, int newQuantity, decimal newPrice, string note);
        Task<bool> DeleteOrderDetail(int orderDetailId);

        Task<bool> SaveChanges();
        //Task<IEnumerable<OrderDetail>> GetAllWithDetailsAsync();
    }
}
