using BussinessObject;
using DataAccess.Models;
using DataAccess.Repository.Base;
using DataAccess.Repository.role;

using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinessObject.role
{
    public class RoleService : BaseService<Role>, IRoleService
    {
        private readonly IRoleRepository _roleRepository;
        public RoleService(IUnitOfWork unitOfWork, IRoleRepository roleRepository) : base(unitOfWork)
        {
            _roleRepository = roleRepository;
        }

       


    }
}
