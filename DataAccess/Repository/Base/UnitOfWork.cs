using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository.Base
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly MenuQContext _context;

        public UnitOfWork(MenuQContext context)
        {
            _context = context;
        }

        public MenuQContext DataContext => _context;

        public async Task BeginTransactionAsync()
        {
            await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            await _context.Database.CommitTransactionAsync();
        }

        public async Task RollbackTransactionAsync()
        {
            await _context.Database.RollbackTransactionAsync();
        }

        public int SaveChanges()
        {
            return _context.SaveChanges();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        // Các repository cho từng entity
        private IBaseRepository<Account>? _accountRepository;
        public IBaseRepository<Account> AccountRepository =>
            _accountRepository ??= new BaseRepository<Account>(_context);

        private IBaseRepository<Admin>? _adminRepository;
        public IBaseRepository<Admin> AdminRepository =>
            _adminRepository ??= new BaseRepository<Admin>(_context);

        private IBaseRepository<Area>? _areaRepository;
        public IBaseRepository<Area> AreaRepository =>
            _areaRepository ??= new BaseRepository<Area>(_context);

        private IBaseRepository<CancellationReason>? _cancellationReasonRepository;
        public IBaseRepository<CancellationReason> CancellationReasonRepository =>
            _cancellationReasonRepository ??= new BaseRepository<CancellationReason>(_context);

        private IBaseRepository<Category>? _categoryRepository;
        public IBaseRepository<Category> CategoryRepository =>
            _categoryRepository ??= new BaseRepository<Category>(_context);

        private IBaseRepository<Customer>? _customerRepository;
        public IBaseRepository<Customer> CustomerRepository =>
            _customerRepository ??= new BaseRepository<Customer>(_context);

        private IBaseRepository<Employee>? _employeeRepository;
        public IBaseRepository<Employee> EmployeeRepository =>
            _employeeRepository ??= new BaseRepository<Employee>(_context);

        private IBaseRepository<MenuItem>? _menuItemRepository;
        public IBaseRepository<MenuItem> MenuItemRepository =>
            _menuItemRepository ??= new BaseRepository<MenuItem>(_context);

        private IBaseRepository<OperatingHour>? _operatingHourRepository;
        public IBaseRepository<OperatingHour> OperatingHourRepository =>
            _operatingHourRepository ??= new BaseRepository<OperatingHour>(_context);

        private IBaseRepository<OrderDetail>? _orderDetailRepository;
        public IBaseRepository<OrderDetail> OrderDetailRepository =>
            _orderDetailRepository ??= new BaseRepository<OrderDetail>(_context);

        private IBaseRepository<Request>? _requestRepository;
        public IBaseRepository<Request> RequestRepository =>
            _requestRepository ??= new BaseRepository<Request>(_context);

        private IBaseRepository<RequestStatus>? _requestStatusRepository;
        public IBaseRepository<RequestStatus> RequestStatusRepository =>
            _requestStatusRepository ??= new BaseRepository<RequestStatus>(_context);

        private IBaseRepository<RequestType>? _requestTypeRepository;
        public IBaseRepository<RequestType> RequestTypeRepository =>
            _requestTypeRepository ??= new BaseRepository<RequestType>(_context);

        private IBaseRepository<Role>? _roleRepository;
        public IBaseRepository<Role> RoleRepository =>
            _roleRepository ??= new BaseRepository<Role>(_context);

        private IBaseRepository<ServiceCall>? _serviceCallRepository;
        public IBaseRepository<ServiceCall> ServiceCallRepository =>
            _serviceCallRepository ??= new BaseRepository<ServiceCall>(_context);

        private IBaseRepository<ServiceReason>? _serviceReasonRepository;
        public IBaseRepository<ServiceReason> ServiceReasonRepository =>
            _serviceReasonRepository ??= new BaseRepository<ServiceReason>(_context);

        private IBaseRepository<Table>? _tableRepository;
        public IBaseRepository<Table> TableRepository =>
            _tableRepository ??= new BaseRepository<Table>(_context);

        MenuQContext IUnitOfWork.DataContext => throw new NotImplementedException();

        // Base Repository Generic
       

        // Dispose
        public void Dispose()
        {
            _context.Dispose();
        }

        public IBaseRepository<TEntity> BaseRepository<TEntity>() where TEntity : class
        {
            return new BaseRepository<TEntity>(_context);
        }
    }
}
