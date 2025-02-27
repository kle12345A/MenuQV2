using BussinessObject.area;
using BussinessObject;
using DataAccess.Models;
using DataAccess.Repository.area;
using DataAccess.Repository.Base;
using Microsoft.EntityFrameworkCore;

public class AreaService : BaseService<Area>, IAreaService
{
    private readonly IAreaRepository _areaRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AreaService(IUnitOfWork unitOfWork, IAreaRepository areaRepository) : base(unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _areaRepository = areaRepository;
    }

    public async Task AddAreaAsync(Area area)
    {
        if (area == null || string.IsNullOrWhiteSpace(area.AreaName))
        {
            throw new ArgumentException("Tên khu vực không được để trống.");
        }

        var newArea = new Area
        {
            AreaName = area.AreaName,
            Status = true,
        };

        await _areaRepository.AddAsync(newArea);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<List<Area>> GetAllAreasWithTablesAsync()
    {
        return await _areaRepository.GetQuery().Include(a => a.Tables).ToListAsync();
    }

    public async Task<List<Table>> GetTablesByAreaIdAsync(int areaId, int tableCount)
    {
        var area = await _areaRepository.GetQuery()
            .Include(a => a.Tables)
            .FirstOrDefaultAsync(a => a.AreaId == areaId);

        if (area == null)
        {
            throw new ArgumentException("Khu vực không tồn tại.");
        }

        return area.Tables.Take(tableCount).ToList();
    }

}
