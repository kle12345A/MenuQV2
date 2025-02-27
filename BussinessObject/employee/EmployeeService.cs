using BussinessObject;
using DataAccess.Models;
using DataAccess.Repository.Base;
using DataAccess.Repository.employee;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BussinessObject.employee
{
    public class EmployeeService : BaseService<Employee>, IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;

        public EmployeeService(IUnitOfWork unitOfWork, IEmployeeRepository employeeRepository) : base(unitOfWork)
        {
            _employeeRepository = employeeRepository;
        }

        public async Task<IEnumerable<Employee>> GetAllEmployee()
        {
            return await _employeeRepository.GetAll()
                .Include(e => e.Account)
                .ToListAsync();
        }

        public async Task<int> AddAsync(Employee employee)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var newEmployee = new Employee
                {
                    EmployeeId = employee.EmployeeId,
                    AccountId = employee.AccountId,
                    FullName = employee.FullName,
                    Position = employee.Position,
                    HireDate = employee.HireDate
                };

                await _employeeRepository.AddAsync(newEmployee);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                return 1; // Thành công
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task<int> UpdateAsync(Employee employee)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var existingEmployee = await _employeeRepository.GetAll()
                    .FirstOrDefaultAsync(e => e.EmployeeId == employee.EmployeeId);

                if (existingEmployee == null)
                {
                    return 0; // Không tìm thấy nhân viên
                }

                // Cập nhật thông tin nhân viên
                existingEmployee.AccountId = employee.AccountId;
                existingEmployee.FullName = employee.FullName;
                existingEmployee.Position = employee.Position;
                existingEmployee.HireDate = employee.HireDate;

                await _employeeRepository.UpdateAsync(existingEmployee);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                return 1; // Thành công
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }
    }
}
