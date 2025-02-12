using DataAccess.Models;
using DataAccess.Repository.Base;

namespace DataAccess.Repository.role
{
    public class RoleRepository : BaseRepository<Role>, IRoleRepository
    {
        public RoleRepository(MenuQContext context) : base(context)
        {
        }
    }
}
