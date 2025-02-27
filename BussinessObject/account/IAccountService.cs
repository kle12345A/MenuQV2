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
        Task<Account?> LoginAsync(string username, string password);
        Task<bool> IsAccountExists(string username, string email);
        Task<Account?> GetByEmailAsync(string email);
        Task<int> AddWithDetailsAsync(Account accountModel, Employee? employee, Admin? admin);
        

        Task<int> AddAsync(Account accountmodel);
        Task<int> UpdateAccountAsync(Account accountmodel, int id);
        Task<IEnumerable<Account>> GetAllAccount();
        Task<int> DeleteAsync(int id);
    }
}
