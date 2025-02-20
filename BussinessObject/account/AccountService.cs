using BussinessObject;
using DataAccess.Models;
using DataAccess.Repository.account;
using DataAccess.Repository.Base;
using DataAccess.Repository.role;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinessObject.account
{
    public class AccountService : BaseService<Account>, IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IRoleRepository _roleRepository;

        public AccountService(IUnitOfWork unitOfWork, IAccountRepository accountRepository, IRoleRepository roleRepository) : base(unitOfWork)
        {
            _roleRepository = roleRepository;
            _accountRepository = accountRepository;
        }

        public async Task<int> AddAsync(Account accountModel)
        {
            if (await IsAccountExists(accountModel.UserName, accountModel.Email))
            {
                return -1;
            }

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var newAccount = new Account
                {
                    AccountId = accountModel.AccountId,
                    UserName = accountModel.UserName,
                    Email = accountModel.Email,
                    Password = accountModel.Password,
                    PhoneNumber = accountModel.PhoneNumber,
                    RoleId = accountModel.RoleId,
                    Active = accountModel.Active,
                    CreatedAt = DateTime.UtcNow,
                };

                await _accountRepository.AddAsync(newAccount);
                await _unitOfWork.SaveChangesAsync();

                await _unitOfWork.CommitTransactionAsync();
                return 1;
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task<Account?> GetByEmailAsync(string email)
        {
            return await Task.Run(() =>
            {
                return _accountRepository.GetQuery()
                    .FirstOrDefault(a => a.Email == email);
            });
        }

        public async Task<bool> IsAccountExists(string username, string email)
        {
            return await _accountRepository.GetQuery()
                .AnyAsync(a => a.UserName.ToLower() == username.ToLower() || a.Email.ToLower() == email.ToLower());
        }

        public Account? Login(string email, string password)
        {
            var account = _accountRepository.GetByEmail(email);

            if (account == null || account.Password != password)
            {
                return null;
            }

            return account;
        }

        public async Task<int> UpdateAccountAsync(Account accountmodel, int id)
        {
            var existingAccount = await _accountRepository.GetByIdAsync(id);
            if (existingAccount == null)
            {
                return -1;
            }
            existingAccount.Email = accountmodel.Email;
            existingAccount.UserName = accountmodel.UserName;
            existingAccount.PhoneNumber = accountmodel.PhoneNumber;
            existingAccount.Password = accountmodel.Password;
            existingAccount.Active = accountmodel.Active;
            existingAccount.RoleId = accountmodel.RoleId;
            await _accountRepository.UpdateAsync(existingAccount);
            await _unitOfWork.SaveChangesAsync();

            return 1;
        }

        public async Task<int> DeleteAsync(int id)
        {
            var existingAccount = await _accountRepository.GetByIdAsync(id);
            if (existingAccount == null)
            {
                return -1;
            }

            await _accountRepository.DeleteAsync(existingAccount);
            await _unitOfWork.SaveChangesAsync();

            return 1;
        }

        public async Task<IEnumerable<Account>> GetAllAccount()
        {
            var acc = await _accountRepository.GetAll();
            return acc;
        }

        public async Task<int> AddWithDetailsAsync(Account accountModel, Employee? employee, Admin? admin)
        {
            if (await IsAccountExists(accountModel.UserName, accountModel.Email))
            {
                return -1; // Tài khoản đã tồn tại
            }

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var newAccount = new Account
                {
                    UserName = accountModel.UserName,
                    Email = accountModel.Email,
                    Password = accountModel.Password, // Cần mã hóa mật khẩu
                    PhoneNumber = accountModel.PhoneNumber,
                    RoleId = accountModel.RoleId,
                    Active = accountModel.Active,
                    CreatedAt = DateTime.UtcNow,
                };

                await _accountRepository.AddAsync(newAccount);
                await _unitOfWork.SaveChangesAsync(); // Lưu trước để có AccountId

                if (employee != null)
                {
                    employee.AccountId = newAccount.AccountId;
                    await _unitOfWork.EmployeeRepository.AddAsync(employee);
                }
                else if (admin != null)
                {
                    admin.AccountId = newAccount.AccountId;
                    await _unitOfWork.AdminRepository.AddAsync(admin);
                }

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();
                return 1;
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }
    }
}
