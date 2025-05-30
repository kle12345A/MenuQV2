﻿using DataAccess.Models;
using DataAccess.Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository.servicereason
{
    public interface IServiceReasonRepository : IBaseRepository<ServiceReason>
    {
        Task<List<ServiceReason>> GetAllActive();

        Task<int> GetReasonDefaultId();
    }
}
