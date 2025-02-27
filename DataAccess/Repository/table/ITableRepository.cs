using DataAccess.Models;
using DataAccess.Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository.table
{
    public interface ITableRepository : IBaseRepository<Table>
    {
        Task<Table> GetTableByIdAsync(int id);
    }
}
