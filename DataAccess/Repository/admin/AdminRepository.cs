using DataAccess.Models;
using DataAccess.Repository.Base;

namespace DataAccess.Repository.admin
{
    public class AdminRepository : BaseRepository<Admin>, IAdminRepository
    {
        public AdminRepository(MenuQContext context) : base(context)
        {
        }
    }
}

