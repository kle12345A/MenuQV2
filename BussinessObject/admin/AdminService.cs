using BussinessObject;
using DataAccess.Models;
using DataAccess.Repository.admin;
using DataAccess.Repository.Base;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BussinessObject.admin
{
    public class AdminService : BaseService<Admin>, IAdminService
    {
        private readonly IAdminRepository _adminRepository;

        public AdminService(IUnitOfWork unitOfWork, IAdminRepository adminRepository) : base(unitOfWork)
        {
            _adminRepository = adminRepository;
        }

      
       
    }
}
