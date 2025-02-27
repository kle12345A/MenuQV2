using DataAccess.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BussinessObject.area
{
    public interface IAreaService : IBaseService<Area>
    {
        Task<List<Area>> GetAllAreasWithTablesAsync();
        Task<List<Table>> GetTablesByAreaIdAsync(int areaId, int tableCount);
        Task AddAreaAsync(Area area);
    }
}
