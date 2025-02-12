using BussinessObject;
using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinessObject.account
{
    public interface IAccountService : IBaseService<Account>
    {
        Account? Login(string email, string password);
        
        Task<bool> IsAccountExists(string username, string email);
        Task<Account?> GetByEmailAsync(string email);

        Task<int> AddAsync(Account accountmodel);
        Task<int> UpdateAsync(Account accountmodel, int id);
        Task<int> DeleteAsync(int id);
    }
}
