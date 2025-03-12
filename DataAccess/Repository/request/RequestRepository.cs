﻿using DataAccess.Enum;
using DataAccess.Models;
using DataAccess.Repository.Base;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repository.request
{
    public class RequestRepository : BaseRepository<Request>, IRequestRepository
    {
        private readonly MenuQContext _context;

        public RequestRepository(MenuQContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Request?> GetCustomerServingRequest(int customerId)
        {
            return await _context.Requests
                .AsNoTracking()
                .Where(r => r.CustomerId == customerId && r.RequestTypeId == 1 && r.RequestStatusId == 3)
                .OrderByDescending(r => r.CreatedAt)
                .FirstOrDefaultAsync();
        }

        public async Task<Request?> AddNewRequest(Request request)
        {
            try
            {
                var createdRequest = await _context.Requests.AddAsync(request);
                await _context.SaveChangesAsync(); // 🟢 Lưu vào database ngay lập tức

                return createdRequest.Entity; // 🟢 Trả về Request vừa tạo
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error adding new request.");
                return null;
            }
        }


        public async Task<List<Request>> GetCustomerInProcessRequests(int customerId, int? accountId = null)
        {
            return await _context.Requests
            .Where(r => r.CustomerId == customerId &&
                       r.RequestStatusId == 2 &&
                       r.RequestTypeId == 1 &&
                       r.AccountId == accountId)
            .ToListAsync();
        }

        public async Task<Request> GetCheckoutRequestByCustomer(int customerId)
        {
            return await _context.Requests
                .Where(r => r.CustomerId == customerId && r.RequestTypeId == 3
                    && (r.RequestStatusId == 1 || r.RequestStatusId == 2))
                .OrderByDescending(r => r.CreatedAt)
                .FirstOrDefaultAsync();
        }


        public async Task<List<Request>> GetPendingRequests(string type = "All")
        {
            var query = _context.Requests
            .AsNoTracking()
            .Where(r => r.AccountId == null && r.RequestStatusId == 1)
            .Include(r => r.Table)
            .Include(r => r.Customer)
            .Include(r => r.RequestType)
            .Include(r => r.RequestStatus)
            .Include(r => r.OrderDetails)
                .ThenInclude(od => od.Item)
            .AsQueryable();

            if (type != "All")
            {
                query = query.Where(r => r.RequestType.RequestTypeName == type);
            }

            return await query.ToListAsync();
        }

        public async Task<Request?> GetRequestById(int requestId)
        {
            return await _context.Requests
                .Include(r => r.Customer)
                .Include(r => r.Table)
                .Include(r => r.RequestType)
                .Include(r => r.RequestStatus)
                .Include(r => r.OrderDetails)
                    .ThenInclude(od => od.Item)
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.RequestId == requestId);
        }

        public async Task<Request> GetLatestRequestByCustomer(int customerId)
        {
            return await _context.Requests
                .Where(r => r.CustomerId == customerId)
                .OrderByDescending(r => r.CreatedAt)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> RejectRequest(int requestId, int reasonId, int? accountId = null)
        {
            var request = await _context.Requests.FindAsync(requestId);
            if (request == null || request.RequestStatusId != 1) return false;

            var reasonExists = await _context.CancellationReasons
                .AnyAsync(r => r.ReasonId == reasonId && r.Status == true);
            if (!reasonExists) return false;

            request.RequestStatusId = 4; // Hủy yêu cầu
            request.CancellationReasonId = reasonId;
            request.AccountId = accountId;

            return await SaveChanges();
        }


        public async Task<bool> UpdateRequestStatus(int requestId, int newStatusId, int? accountId = null, int? cancellationReasonId = null)
        {
            var request = await _context.Requests.FindAsync(requestId);
            if (request == null) return false;

            request.RequestStatusId = newStatusId;
            if (accountId.HasValue) request.AccountId = accountId.Value;
            if (cancellationReasonId.HasValue) request.CancellationReasonId = cancellationReasonId.Value;

            return await SaveChanges();
        }




        public async Task<bool> MarkRequestInProcess(int requestId, int? accountId = null)
        {
            var request = await _context.Requests.FindAsync(requestId);
            if (request == null || request.RequestStatusId != 1) return false;

            request.RequestStatusId = 2; // Chuyển trạng thái "InProcess"
            request.AccountId = accountId;
            return await SaveChanges();
        }


        public async Task<bool> ResetPendingRequest(int requestId)
        {
            var request = await _context.Requests.FindAsync(requestId);
            if (request == null || request.RequestStatusId == 3 || request.RequestStatusId == 4) return false;
            if (request.RequestStatusId != 2) return false;

            request.RequestStatusId = 1; // Reset về Pending
            request.AccountId = null;

            return await SaveChanges();
        }

        public async Task<Request?> GetLatestAcceptedRequest(int customerId)
        {
            return await _context.Requests
                .AsNoTracking()
                .Where(r => r.CustomerId == customerId && r.RequestTypeId == 1 && r.RequestStatusId == 3)
                .OrderByDescending(r => r.CreatedAt)
                .FirstOrDefaultAsync();
        }

        public async Task<List<Request>> GetAcceptedOrdersAsync(string filter = "Accepted")
        {
            var query = _context.Requests
               .AsNoTracking()
               .Where(r => r.RequestTypeId == 1 && r.RequestStatusId == 3)
               .Include(r => r.Table)
                   .ThenInclude(t => t.Area)
               .Include(r => r.Customer)
               .Include(r => r.RequestStatus)
               .Include(r => r.OrderDetails)
               .AsQueryable();

            if (filter == "Today")
            {
                var today = DateTime.Today;
                query = query.Where(r => r.CreatedAt.HasValue && r.CreatedAt.Value.Date == today);
            }

            return await query.ToListAsync();
        }

        public async Task LoadRequestRelations(Request request)
        {
            if (!_context.Entry(request).Reference(r => r.Table).IsLoaded)
                await _context.Entry(request).Reference(r => r.Table).LoadAsync();

            if (!_context.Entry(request).Reference(r => r.Customer).IsLoaded)
                await _context.Entry(request).Reference(r => r.Customer).LoadAsync();

            if (!_context.Entry(request).Reference(r => r.RequestType).IsLoaded)
                await _context.Entry(request).Reference(r => r.RequestType).LoadAsync();

            if (!_context.Entry(request).Reference(r => r.RequestStatus).IsLoaded)
                await _context.Entry(request).Reference(r => r.RequestStatus).LoadAsync();

            if (!_context.Entry(request).Collection(r => r.OrderDetails).IsLoaded)
            {
                await _context.Entry(request).Collection(r => r.OrderDetails).LoadAsync();
                foreach (var orderDetail in request.OrderDetails)
                {
                    if (!_context.Entry(orderDetail).Reference(od => od.Item).IsLoaded)
                        await _context.Entry(orderDetail).Reference(od => od.Item).LoadAsync();
                }
            }
        }

        public async Task<bool> SaveChanges()
        {
            try
            {
                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] SaveChanges failed: {ex.Message}");
                return false;
            }
        }

        public async Task<Request> GetServingFoodOrderRequest(int customerId)
        {
            return await _context.Requests
                .Include(r => r.Customer)
                .Include(r => r.RequestStatus)
                .Include(r => r.Table)
                .Include(r => r.OrderDetails)
                    .ThenInclude(od => od.Item)
                .Where(r => r.CustomerId == customerId && r.RequestTypeId == 1
                    && (r.RequestStatusId == 1 || r.RequestStatusId == 2 || r.RequestStatusId == 3) // Chỉ lấy Pending, Processing hoặc Accepted
                    && _context.Invoices.Any(i => i.RequestId == r.RequestId && (i.InvoiceStatus == InvoiceStatus.Serving || i.InvoiceStatus == InvoiceStatus.ProcessingPayment))) // Kiểm tra hóa đơn chưa thanh toán
                .OrderByDescending(r => r.CreatedAt) // Lấy request mới nhất
                .FirstOrDefaultAsync();
        }



    }

}