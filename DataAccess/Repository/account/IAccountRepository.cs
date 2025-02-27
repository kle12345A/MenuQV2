using DataAccess.Models;
using DataAccess.Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository.account
{
    public interface IAccountRepository : IBaseRepository<Account>
    {
        //muốn thêm phương thức mới thì viết ở đây và triển khai nó ở AccountRepository
        //vdu
        Account GetByEmail(string email);
        Task<IEnumerable<Account>> GetAll();    
    }
}
