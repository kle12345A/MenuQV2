using BussinessObject;
using DataAccess.Models;
using DataAccess.Repository.Base;
using DataAccess.Repository.operatinghour;

using System;
using System.Threading.Tasks;

namespace BussinessObject.operatinghour
{
    public class OperatingHourService : BaseService<OperatingHour>, IOperatingHourService
    {
        private readonly IOperatingHourRepository _operatingHourRepository;

        public OperatingHourService(IUnitOfWork unitOfWork, IOperatingHourRepository operatingHourRepository)
            : base(unitOfWork)
        {
            _operatingHourRepository = operatingHourRepository;
        }

       
    }
}
