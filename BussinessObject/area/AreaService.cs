
using BussinessObject;
using DataAccess.Models;
using DataAccess.Repository.area;
using DataAccess.Repository.Base;
using System;
using System.Threading.Tasks;

namespace BussinessObject.area
{
    public class AreaService : BaseService<Area>, IAreaService
    {
        private readonly IAreaRepository _areaRepository;

        public AreaService(IUnitOfWork unitOfWork, IAreaRepository areaRepository) : base(unitOfWork)
        {
            _areaRepository = areaRepository;
        }

        
    }
}
