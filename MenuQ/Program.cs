using BussinessObject.Dto;
using BussinessObject.account;
using BussinessObject.admin;
using BussinessObject.area;
using BussinessObject.cancellreason;
using BussinessObject.category;
using BussinessObject.customer;
using BussinessObject.employee;
using BussinessObject.menu;
using BussinessObject.operatinghour;
using BussinessObject.orderdetail;
using BussinessObject.request;
using BussinessObject.requeststatus;
using BussinessObject.requesttype;
using BussinessObject.role;
using BussinessObject.servicecall;
using BussinessObject.servicereason;
using BussinessObject.table;
using DataAccess.Models;
using DataAccess.Repository.account;
using DataAccess.Repository.admin;
using DataAccess.Repository.area;
using DataAccess.Repository.Base;
using DataAccess.Repository.cancellation;
using DataAccess.Repository.category;
using DataAccess.Repository.customer;
using DataAccess.Repository.employee;
using DataAccess.Repository.menuitem;
using DataAccess.Repository.operatinghour;
using DataAccess.Repository.orderdetail;
using DataAccess.Repository.request;
using DataAccess.Repository.requeststatus;
using DataAccess.Repository.requesttype;
using DataAccess.Repository.role;
using DataAccess.Repository.servicecall;
using DataAccess.Repository.servicereason;
using DataAccess.Repository.table;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

// Cấu hình DbContext với SQL Server
builder.Services.AddDbContext<MenuQContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MenuQDB")));
services.AddScoped<IAccountRepository, AccountRepository>();
services.AddScoped<IRoleRepository, RoleRepository>();
services.AddScoped<IAdminRepository, AdminRepository>();
services.AddScoped<IAreaRepository, AreaRepository>();
services.AddScoped<ICategoryRepository, CategoryRepository>();
services.AddScoped<ICustomerRepository, CustomerRepository>();
services.AddScoped<IEmployeeRepository, EmployeeRepository>();
services.AddScoped<IMenuItemRepository, MenuItemRepository>();
services.AddScoped<IOperatingHourRepository, OperatingHourRepository>();

// Đăng ký các repository bị thiếu
services.AddScoped<ICancellationReasonRepository, CancellationReasonRepository>();
services.AddScoped<IOrderDetailRepository, OrderDetailRepository>();
services.AddScoped<IRequestRepository, RequestRepository>();
services.AddScoped<IRequestStatusRepository, RequestStatusRepository>();
services.AddScoped<IRequestTypeRepository, RequestTypeRepository>();
services.AddScoped<IServiceCallRepository, ServiceCallRepository>();
services.AddScoped<IServiceReasonRepository, ServiceReasonRepository>();
services.AddScoped<ITableRepository, TableRepository>();

services.AddScoped<ICancellReasonService, CancellReasonService>();
services.AddScoped<IEmployeeService, EmployeeService>();
services.AddScoped<IOperatingHourService, OperatingHourService>();
services.AddScoped<IMenuItemService, MenuItemService>();
services.AddScoped<ICategoryService, CategoryService>();
services.AddScoped<ICustomerService, CustomerService>();
services.AddScoped<IAccountService, AccountService>();
services.AddScoped<IAdminService, AdminService>();
services.AddScoped<IRoleService, RoleService>();
services.AddScoped<IAreaService, AreaService>();
services.AddScoped<IOrderDetailService, OrderDetailService>();
services.AddScoped<IRequestService, RequestService>();
services.AddScoped<IRequestStatusService, RequestStatusService>();
services.AddScoped<IRequestTypeService, RequestTypeService>();
services.AddScoped<IServiceCallService, ServiceCallService>();
services.AddScoped<IServiceReasonService, ServiceReasonService>();
services.AddScoped<ITableService, TableService>();

services.AddScoped<IUnitOfWork, UnitOfWork>();
services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));

// Add services to the container.
builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
   