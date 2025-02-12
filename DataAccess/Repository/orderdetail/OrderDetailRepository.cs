using DataAccess.Models;
using DataAccess.Repository.Base;

namespace DataAccess.Repository.orderdetail
{
    public class OrderDetailRepository : BaseRepository<OrderDetail>, IOrderDetailRepository
    {
        public OrderDetailRepository(MenuQContext context) : base(context)
        {
        }
    }
}
