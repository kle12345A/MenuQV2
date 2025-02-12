using DataAccess.Models;
using DataAccess.Repository.Base;

namespace DataAccess.Repository.account
{
    public class AccountRepository : BaseRepository<Account>, IAccountRepository
    {
        private readonly MenuQContext _context;
        public AccountRepository(MenuQContext context) : base(context)
        {
            _context = context;
        }

        //vidu
        public Account GetByEmail(string email)
        {
            return _context.Accounts.FirstOrDefault(a => a.Email == email);
        }
    }
}
