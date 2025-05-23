﻿using DataAccess.Models;
using DataAccess.Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository.cancellation
{
    public interface ICancellationReasonRepository : IBaseRepository<CancellationReason>
    {
        Task<List<CancellationReason>> GetActiveCancellationReasons();
        Task<CancellationReason?> GetCancellationReasonById(int reasonId);

    }
}
