

using BussinessObject;
using DataAccess.Models;
using DataAccess.Repository.Base;
using DataAccess.Repository.customer;
using MenuQ.Models;
using Microsoft.Extensions.Logging;

namespace BussinessObject.customer
{
    public class CustomerService : BaseService<Customer>, ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly ILogger<CustomerService> _logger;

        public CustomerService(IUnitOfWork unitOfWork,
                               ICustomerRepository customerRepository,
                               ILogger<CustomerService> logger) : base(unitOfWork)
        {
            _customerRepository = customerRepository;
            _logger = logger;
        }

        public async Task<List<OrderHistory>> GetOrderHistoryByCustomerId(int customerId)
        {
            return await _customerRepository.GetOrderHistoryByCustomerId(customerId);
        }
        public async Task<Customer> GetCustomerByPhone(string Phone)
        {
            return await _customerRepository.getCustomerByPhone(Phone);
        }

        public async Task<Customer> CustomerLogin(string Phone, string username)
        {
            var customer = await _customerRepository.getCustomerByPhone(Phone);
            if (customer == null)
            {
                customer = new Customer
                {
                    CustomerName = username,
                    PhoneNumber = Phone,
                    CreatedAt = DateTime.Now,
                };
                await _customerRepository.AddAsync(customer);
            }
            else
            {
                if(customer.CustomerName != username)
                {
                    customer.CustomerName = username;
                }
                await _customerRepository.UpdateAsync(customer);
            }
            return customer;
        }
    }
}

        // Tính số lượng khách hàng ngày hôm nay
        public async Task<int> CalculateTotalCustomersForTodayAsync()
        {
            try
            {
                var customers = await _customerRepository.GetAllAsync();
                var today = DateTime.UtcNow.Date; // Hoặc DateTime.Now.Date nếu CreatedAt lưu giờ địa phương

                var todayCustomers = customers
                    .Where(c => c.CreatedAt.HasValue && c.CreatedAt.Value.Date == today)
                    .ToList();

                int totalCustomers = todayCustomers.Count;

                _logger.LogInformation("Successfully calculated total customers for today {Today}: {TotalCustomers}",
                    today.ToString("yyyy-MM-dd"), totalCustomers);
                return totalCustomers;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating total customers for today");
                throw;
            }
        }

        // Tính số lượng khách hàng tháng hiện tại
        public async Task<int> CalculateTotalCustomersForCurrentMonthAsync()
        {
            try
            {
                var customers = await _customerRepository.GetAllAsync();
                var currentDate = DateTime.UtcNow;
                var currentMonth = currentDate.Month;
                var currentYear = currentDate.Year;

                var monthlyCustomers = customers
                    .Where(c => c.CreatedAt.HasValue &&
                                c.CreatedAt.Value.Month == currentMonth &&
                                c.CreatedAt.Value.Year == currentYear)
                    .ToList();

                int totalCustomers = monthlyCustomers.Count;

                _logger.LogInformation("Successfully calculated total customers for {Year}-{Month}: {TotalCustomers}",
                    currentYear, currentMonth, totalCustomers);
                return totalCustomers;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating total customers for current month");
                throw;
            }
        }

        // Tính số lượng khách hàng năm hiện tại
        public async Task<int> CalculateTotalCustomersForCurrentYearAsync()
        {
            try
            {
                var customers = await _customerRepository.GetAllAsync();
                var currentYear = DateTime.UtcNow.Year;

                var yearlyCustomers = customers
                    .Where(c => c.CreatedAt.HasValue && c.CreatedAt.Value.Year == currentYear)
                    .ToList();

                int totalCustomers = yearlyCustomers.Count;

                _logger.LogInformation("Successfully calculated total customers for current year {Year}: {TotalCustomers}",
                    currentYear, totalCustomers);
                return totalCustomers;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating total customers for current year");
                throw;
            }
        }

        // Tính số lượng khách hàng ngày hôm qua
        public async Task<int> CalculateTotalCustomersForYesterdayAsync()
        {
            try
            {
                var customers = await _customerRepository.GetAllAsync();
                var yesterday = DateTime.UtcNow.Date.AddDays(-1);

                var yesterdayCustomers = customers
                    .Where(c => c.CreatedAt.HasValue && c.CreatedAt.Value.Date == yesterday)
                    .ToList();

                int totalCustomers = yesterdayCustomers.Count;

                _logger.LogInformation("Successfully calculated total customers for yesterday {Yesterday}: {TotalCustomers}",
                    yesterday.ToString("yyyy-MM-dd"), totalCustomers);
                return totalCustomers;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating total customers for yesterday");
                throw;
            }
        }

        // Tính số lượng khách hàng tháng trước
        public async Task<int> CalculateTotalCustomersForLastMonthAsync()
        {
            try
            {
                var customers = await _customerRepository.GetAllAsync();
                var currentDate = DateTime.UtcNow;
                var lastMonthDate = currentDate.AddMonths(-1);
                var lastMonth = lastMonthDate.Month;
                var lastMonthYear = lastMonthDate.Year;

                var lastMonthCustomers = customers
                    .Where(c => c.CreatedAt.HasValue &&
                                c.CreatedAt.Value.Month == lastMonth &&
                                c.CreatedAt.Value.Year == lastMonthYear)
                    .ToList();

                int totalCustomers = lastMonthCustomers.Count;

                _logger.LogInformation("Successfully calculated total customers for last month {Year}-{Month}: {TotalCustomers}",
                    lastMonthYear, lastMonth, totalCustomers);
                return totalCustomers;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating total customers for last month");
                throw;
            }
        }

        // Tính số lượng khách hàng năm trước
        public async Task<int> CalculateTotalCustomersForLastYearAsync()
        {
            try
            {
                var customers = await _customerRepository.GetAllAsync();
                var lastYear = DateTime.UtcNow.Year - 1;

                var lastYearCustomers = customers
                    .Where(c => c.CreatedAt.HasValue && c.CreatedAt.Value.Year == lastYear)
                    .ToList();

                int totalCustomers = lastYearCustomers.Count;

                _logger.LogInformation("Successfully calculated total customers for last year {Year}: {TotalCustomers}",
                    lastYear, totalCustomers);
                return totalCustomers;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating total customers for last year");
                throw;
            }
        }
            public async Task<Customer> CustomerLogin(string Phone, string username)
            {
                var customer = await _customerRepository.getCustomerByPhone(Phone);
                if (customer == null)
                {
                    customer = new Customer
                    {
                        CustomerName = username,
                        PhoneNumber = Phone,
                        CreatedAt = DateTime.Now,
                    };
                    await _customerRepository.AddAsync(customer);
                }
                return customer;

            }
        }
    }
