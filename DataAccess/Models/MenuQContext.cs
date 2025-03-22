using System;
using System.Collections.Generic;
using DataAccess.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace DataAccess.Models;

public partial class MenuQContext : DbContext
{
    public MenuQContext()
    {
    }

    public MenuQContext(DbContextOptions<MenuQContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<Admin> Admins { get; set; }

    public virtual DbSet<Area> Areas { get; set; }

    public virtual DbSet<CancellationReason> CancellationReasons { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<Invoice> Invoices { get; set; }

    public virtual DbSet<MenuItem> MenuItems { get; set; }

    public virtual DbSet<OperatingHour> OperatingHours { get; set; }

    public virtual DbSet<OrderDetail> OrderDetails { get; set; }

    public virtual DbSet<Request> Requests { get; set; }

    public virtual DbSet<RequestStatus> RequestStatuses { get; set; }

    public virtual DbSet<RequestType> RequestTypes { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<ServiceCall> ServiceCalls { get; set; }

    public virtual DbSet<ServiceReason> ServiceReasons { get; set; }

    public virtual DbSet<Table> Tables { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {

        if (!optionsBuilder.IsConfigured)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();
            var sqlConnectionStr = configuration.GetConnectionString("MenuQDB");
            optionsBuilder.UseSqlServer(sqlConnectionStr, config =>
            {
                config.EnableRetryOnFailure();
            });
        }
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.AccountId).HasName("PK__Accounts__349DA58636CF0857");

            entity.HasIndex(e => e.Email, "UQ__Accounts__A9D10534B4A7E9FB").IsUnique();

            entity.HasIndex(e => e.UserName, "UQ__Accounts__C9F2845606DD754B").IsUnique();

            entity.Property(e => e.AccountId).HasColumnName("AccountID");
            entity.Property(e => e.Active).HasDefaultValue(true);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .IsUnicode(true);
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.UserName)
                .HasMaxLength(50)
                .IsUnicode(true);

            entity.HasOne(d => d.Role).WithMany(p => p.Accounts)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Accounts__RoleID__1DB06A4F");
        });

        modelBuilder.Entity<Admin>(entity =>
        {
            entity.HasKey(e => e.AdminId).HasName("PK__Admins__719FE4E830B29429");

            entity.HasIndex(e => e.AccountId, "UQ__Admins__349DA5873A01E3F9").IsUnique();

            entity.Property(e => e.AdminId).HasColumnName("AdminID");
            entity.Property(e => e.AccountId).HasColumnName("AccountID");
            entity.Property(e => e.FullName)
                .HasMaxLength(100)
                .IsUnicode(true);
            entity.Property(e => e.Position)
                .HasMaxLength(50)
                .IsUnicode(true);

            entity.HasOne(d => d.Account).WithOne(p => p.Admin)
                .HasForeignKey<Admin>(d => d.AccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Admins__AccountI__1EA48E88");
        });

        modelBuilder.Entity<Area>(entity =>
        {
            entity.HasKey(e => e.AreaId).HasName("PK__Areas__70B82028A927D429");

            entity.Property(e => e.AreaId).HasColumnName("AreaID");
            entity.Property(e => e.AreaName)
                .HasMaxLength(50)
                .IsUnicode(true);
            entity.Property(e => e.Status).HasDefaultValue(true);
        });

        modelBuilder.Entity<CancellationReason>(entity =>
        {
            entity.HasKey(e => e.ReasonId).HasName("PK__Cancella__A4F8C0C758721D6C");

            entity.Property(e => e.ReasonId).HasColumnName("ReasonID");
            entity.Property(e => e.ReasonText)
                .HasMaxLength(200)
                .IsUnicode(true);
            entity.Property(e => e.Status).HasDefaultValue(true);
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__Categori__19093A2B27C0ADE2");

            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.CategoryName)
                .HasMaxLength(100)
                .IsUnicode(true);
            entity.Property(e => e.Status).HasDefaultValue(true);
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.CustomerId).HasName("PK__Customer__A4AE64B832713F8D");

            entity.Property(e => e.CustomerId).HasColumnName("CustomerID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.CustomerName)
                .HasMaxLength(100)
                .IsUnicode(true);
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.EmployeeId).HasName("PK__Employee__7AD04FF16E46D259");

            entity.HasIndex(e => e.AccountId, "UQ__Employee__349DA587A171985F").IsUnique();

            entity.Property(e => e.EmployeeId).HasColumnName("EmployeeID");
            entity.Property(e => e.AccountId).HasColumnName("AccountID");
            entity.Property(e => e.FullName)
                .HasMaxLength(100)
                .IsUnicode(true);
            entity.Property(e => e.Position)
                .HasMaxLength(50)
                .IsUnicode(true);

            entity.HasOne(d => d.Account).WithOne(p => p.Employee)
                .HasForeignKey<Employee>(d => d.AccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Employees__Accou__1F98B2C1");
        });

        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.HasKey(e => e.InvoiceId).HasName("PK__Invoices__D796AAD591F5425C");

            entity.HasIndex(e => e.InvoiceCode, "UQ__Invoices__0D9D7FF30DE1C806").IsUnique();

            entity.Property(e => e.InvoiceId).HasColumnName("InvoiceID");
            entity.Property(e => e.InvoiceCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.PaymentDate).HasColumnType("datetime");
            entity.Property(e => e.PaymentMethod)
                .HasMaxLength(50)
                .IsUnicode(true);
            entity.Property(e => e.RequestId).HasColumnName("RequestID");
            entity.Property(e => e.CustomerId).HasColumnName("CustomerID");
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.TableId).HasColumnName("TableID");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Request).WithMany(p => p.Invoices)
                .HasForeignKey(d => d.RequestId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Invoices__Reques__00200768");

            entity.HasOne(d => d.Customer).WithMany(p => p.Invoices)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Invoices__Custom__6EF57B66");

            entity.Property(e => e.InvoiceStatus)
                .HasConversion<int>()
                .HasDefaultValue(InvoiceStatus.Serving);
        });

        modelBuilder.Entity<MenuItem>(entity =>
        {
            entity.HasKey(e => e.ItemId).HasName("PK__MenuItem__727E83EB3765FFCE");

            entity.Property(e => e.ItemId).HasColumnName("ItemID");
            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.Descriptions).HasColumnType("text");
            entity.Property(e => e.ImageUrl)
                .HasColumnType("text")
                .HasColumnName("ImageURL");
            entity.Property(e => e.ItemName)
                .HasMaxLength(200)
                .IsUnicode(true);
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Status).HasDefaultValue(true);

            entity.HasOne(d => d.Category).WithMany(p => p.MenuItems)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("FK__MenuItems__Categ__01142BA1");
        });

        modelBuilder.Entity<OperatingHour>(entity =>
        {
            entity.HasKey(e => e.OperatingHourId).HasName("PK__Operatin__EDD5E24F11B9E0B5");

            entity.Property(e => e.RestaurantName).HasColumnName("RestaurantName");
            entity.Property(e => e.ImageURL).HasColumnName("ImageURL");
            entity.Property(e => e.OperatingHourId).HasColumnName("OperatingHourID");
            entity.Property(e => e.IsOpen).HasDefaultValue(true);
        });

        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.HasKey(e => e.OrderDetailId).HasName("PK__OrderDet__D3B9D30C712D7228");

            entity.Property(e => e.OrderDetailId).HasColumnName("OrderDetailID");
            entity.Property(e => e.ItemId).HasColumnName("ItemID");
            entity.Property(e => e.Note).HasColumnType("text");
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.RequestId).HasColumnName("RequestID");

            entity.HasOne(d => d.Item).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.ItemId)
                .HasConstraintName("FK__OrderDeta__ItemI__02084FDA");

            entity.HasOne(d => d.Request).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.RequestId)
                .HasConstraintName("FK__OrderDeta__Reque__02FC7413");
        });

        modelBuilder.Entity<Request>(entity =>
        {
            entity.HasKey(e => e.RequestId).HasName("PK__Requests__33A8519A20941F54");

            entity.Property(e => e.RequestId).HasColumnName("RequestID");
            entity.Property(e => e.AccountId).HasColumnName("AccountID");
            entity.Property(e => e.CancellationReasonId).HasColumnName("CancellationReasonID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.CustomerId).HasColumnName("CustomerID");
            entity.Property(e => e.RequestStatusId).HasColumnName("RequestStatusID");
            entity.Property(e => e.RequestTypeId).HasColumnName("RequestTypeID");
            entity.Property(e => e.TableId).HasColumnName("TableID");

            entity.HasOne(d => d.Account).WithMany(p => p.Requests)
                .HasForeignKey(d => d.AccountId)
                .HasConstraintName("FK__Requests__Accoun__03F0984C");

            entity.HasOne(d => d.CancellationReason).WithMany(p => p.Requests)
                .HasForeignKey(d => d.CancellationReasonId)
                .HasConstraintName("FK__Requests__Cancel__04E4BC85");

            entity.HasOne(d => d.Customer).WithMany(p => p.Requests)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK__Requests__Custom__05D8E0BE");

            entity.HasOne(d => d.RequestStatus).WithMany(p => p.Requests)
                .HasForeignKey(d => d.RequestStatusId)
                .HasConstraintName("FK__Requests__Reques__07C12930");

            entity.HasOne(d => d.RequestType).WithMany(p => p.Requests)
                .HasForeignKey(d => d.RequestTypeId)
                .HasConstraintName("FK__Requests__Reques__06CD04F7");

            entity.HasOne(d => d.Table).WithMany(p => p.Requests)
                .HasForeignKey(d => d.TableId)
                .HasConstraintName("FK__Requests__TableI__08B54D69");
        });

        modelBuilder.Entity<RequestStatus>(entity =>
        {
            entity.HasKey(e => e.RequestStatusId).HasName("PK__RequestS__7094B7BB19CEFCF4");

            entity.Property(e => e.RequestStatusId).HasColumnName("RequestStatusID");
            entity.Property(e => e.StatusName).HasMaxLength(10);
        });

        modelBuilder.Entity<RequestType>(entity =>
        {
            entity.HasKey(e => e.RequestTypeId).HasName("PK__RequestT__4D328BA31BC34D03");

            entity.Property(e => e.RequestTypeId).HasColumnName("RequestTypeID");
            entity.Property(e => e.RequestTypeName).HasMaxLength(50);
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Roles__8AFACE3A0D0D1340");

            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.RoleName)
                .HasMaxLength(50)
                .IsUnicode(true);
        });

        modelBuilder.Entity<ServiceCall>(entity =>
        {
            entity.HasKey(e => e.ServiceCallId).HasName("PK__ServiceC__9BA67A595D74E845");

            entity.Property(e => e.ServiceCallId).HasColumnName("ServiceCallID");
            entity.Property(e => e.Note).HasColumnType("text");
            entity.Property(e => e.ReasonId).HasColumnName("ReasonID");
            entity.Property(e => e.RequestId).HasColumnName("RequestID");

            entity.HasOne(d => d.Reason).WithMany(p => p.ServiceCalls)
                .HasForeignKey(d => d.ReasonId)
                .HasConstraintName("FK__ServiceCa__Reaso__09A971A2");

            entity.HasOne(d => d.Request).WithMany(p => p.ServiceCalls)
                .HasForeignKey(d => d.RequestId)
                .HasConstraintName("FK__ServiceCa__Reque__0A9D95DB");
        });

        modelBuilder.Entity<ServiceReason>(entity =>
        {
            entity.HasKey(e => e.ReasonId).HasName("PK__ServiceR__A4F8C0C79E9707CB");

            entity.Property(e => e.ReasonId).HasColumnName("ReasonID");
            entity.Property(e => e.ReasonText)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.Status).HasDefaultValue(true);
        });

        modelBuilder.Entity<Table>(entity =>
        {
            entity.HasKey(e => e.TableId).HasName("PK__Tables__7D5F018ED91C90F1");

            entity.Property(e => e.TableId).HasColumnName("TableID");
            entity.Property(e => e.AreaId).HasColumnName("AreaID");
            entity.Property(e => e.SeatCapacity).HasDefaultValue(4);
            entity.Property(e => e.Status).HasDefaultValue(true);
            entity.Property(e => e.TableNumber)
                .HasMaxLength(10)
                .IsUnicode(true);
            entity.Property(e => e.TableStatus)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Available");

            entity.HasOne(d => d.Area).WithMany(p => p.Tables)
                .HasForeignKey(d => d.AreaId)
                .HasConstraintName("FK__Tables__AreaID__0B91BA14");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
