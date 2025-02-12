using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository.Base
{
    public interface IUnitOfWork : IDisposable
    {
        MenuQContext DataContext { get; }
        int SaveChanges();
        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
        // Repository cho từng entity
        IBaseRepository<Account> AccountRepository { get; }
        IBaseRepository<Admin> AdminRepository { get; }
        IBaseRepository<Area> AreaRepository { get; }
        IBaseRepository<CancellationReason> CancellationReasonRepository { get; }
        IBaseRepository<Category> CategoryRepository { get; }
        IBaseRepository<Customer> CustomerRepository { get; }
        IBaseRepository<Employee> EmployeeRepository { get; }
        IBaseRepository<MenuItem> MenuItemRepository { get; }
        IBaseRepository<OperatingHour> OperatingHourRepository { get; }
        IBaseRepository<OrderDetail> OrderDetailRepository { get; }
        IBaseRepository<Request> RequestRepository { get; }
        IBaseRepository<RequestStatus> RequestStatusRepository { get; }
        IBaseRepository<RequestType> RequestTypeRepository { get; }
        IBaseRepository<Role> RoleRepository { get; }
        IBaseRepository<ServiceCall> ServiceCallRepository { get; }
        IBaseRepository<ServiceReason> ServiceReasonRepository { get; }
        IBaseRepository<Table> TableRepository { get; }
        IBaseRepository<TEntity> BaseRepository<TEntity>() where TEntity : class;


    }
}
