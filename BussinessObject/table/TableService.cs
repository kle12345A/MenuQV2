using BussinessObject;
using DataAccess.Models;
using DataAccess.Repository.Base;
using DataAccess.Repository.table;
using System;
using System.Threading.Tasks;

namespace BussinessObject.table
{
    public class TableService : BaseService<Table>, ITableService
    {
        private readonly ITableRepository _tableRepository;

        public TableService(IUnitOfWork unitOfWork, ITableRepository tableRepository) : base(unitOfWork)
        {
            _tableRepository = tableRepository;
        }

        public async Task<Table> GetTableByIdAsync(int id)
        {
            return await _tableRepository.GetTableByIdAsync(id);
        }
    }
}
