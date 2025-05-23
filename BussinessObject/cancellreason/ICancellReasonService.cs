﻿using BussinessObject;
using DataAccess.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinessObject.cancellreason
{
    public interface ICancellReasonService : IBaseService<CancellationReason>

    {
        Task<List<CancellationReason>> GetActiveCancellationReasons();
        Task<CancellationReason> GetCancellationReasonById(int reasonId);
    }
}
