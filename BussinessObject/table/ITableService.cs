using BussinessObject;
using DataAccess.Models;

using System;
using System.Threading.Tasks;

namespace BussinessObject.table
{
    public interface ITableService : IBaseService<Table>
    {
        Task<Table> GetTableByIdAsync(int id);
    }
}
