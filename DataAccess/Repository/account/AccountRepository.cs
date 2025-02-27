using DataAccess.Models;
using DataAccess.Repository.Base;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repository.account
{
    public class AccountRepository : BaseRepository<Account>, IAccountRepository
    {
        private readonly MenuQContext _context;
        public AccountRepository(MenuQContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Account>> GetAll()
        {
            return await _context.Accounts
                .Include(a => a.Role) 
                .ToListAsync();
        }


        //vidu
        public Account GetByEmail(string email)
        {
            return _context.Accounts.FirstOrDefault(a => a.Email == email);
        }
    }
}
