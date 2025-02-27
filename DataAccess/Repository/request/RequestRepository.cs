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

        public async Task<List<Request>> GetCustomerInProcessRequests(int customerId, int? accountId = null)
        {
            return await _context.Requests
            .Where(r => r.CustomerId == customerId &&
                       r.RequestStatusId == 2 &&
                       r.RequestTypeId == 1 &&
                       r.AccountId == accountId)
            .ToListAsync();
        }

        public async Task<List<Request>> GetPendingRequests(string type = "All")
        {
            var query = _context.Requests
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
        public async Task<Request> GetRequestById(int requestId)
        {
            return await _context.Requests
                .Include(r => r.Customer)
                .Include(r => r.RequestStatus)
                .Include(r => r.OrderDetails)
                    .ThenInclude(od => od.Item)
                .FirstOrDefaultAsync(r => r.RequestId == requestId);
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
        public async Task<bool> RejectRequest(int requestId, int reasonId, int? accountId = null)
        {
            var request = await GetRequestById(requestId);
            var reason = await _context.CancellationReasons
                .FirstOrDefaultAsync(r => r.ReasonId == reasonId && r.Status == true);

            if (request == null || reason == null || request.RequestStatusId != 1)
                return false;

            request.RequestStatusId = 4;
            request.CancellationReasonId = reasonId;
            if (accountId.HasValue)
                request.AccountId = accountId.Value;

            return await SaveChanges();
        }

        public async Task<bool> UpdateRequestStatus(int requestId, int newStatusId, int? accountId = null)
        {
            var request = await GetByIdAsync(requestId);
            if (request == null) return false;

            request.RequestStatusId = newStatusId;
            if (accountId.HasValue)
                request.AccountId = accountId.Value;

            return await SaveChanges();
        }

        public async Task<bool> SaveChanges()
        {
            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving changes: {ex.Message}");
                return false;
            }
        }



        public async Task<bool> MarkRequestInProcess(int requestId, int? accountId = null)
        {
            var request = await GetByIdAsync(requestId);
            if (request == null || request.RequestStatusId != 1)
                return false;

            request.RequestStatusId = 2;
            request.AccountId = accountId;
            return await SaveChanges();
        }

        //public async Task<bool> ResetPendingRequest(int requestId)
        //{
        //    var request = await GetByIdAsync(requestId);
        //    if (request == null || request.RequestStatusId != 2) return false; // Chỉ reset nếu đang InProcess

        //    // Kiểm tra nếu yêu cầu đã được xử lý (Accepted hoặc Rejected) thì không reset
        //    if (request.RequestStatusId == 3 || request.RequestStatusId == 4)
        //        return false;

        //    request.RequestStatusId = 1; // Chuyển lại thành Pending
        //    //request.AccountId = null; // Xóa nhân viên đang xử lý

        //    return await SaveChanges();
        //}

        public async Task<bool> ResetPendingRequest(int requestId)
        {
            var request = await _context.Requests.FirstOrDefaultAsync(r => r.RequestId == requestId);

            if (request == null)
            {
                Console.WriteLine($" Repository: Request ID {requestId} not found in database.");
                return false;
            }

            if (request.RequestStatusId != 2)
            {
                Console.WriteLine($" Repository: Request ID {requestId} is not in 'InProcess' state. Current status: {request.RequestStatusId}");
                return false;
            }

            request.RequestStatusId = 1; // Reset về Pending
            request.AccountId = null; // Xóa nhân viên đang xử lý

            var result = await SaveChanges();

            Console.WriteLine(result ? $" Repository: Request ID {requestId} reset thành công!" : $"Repository: Reset Request ID {requestId} thất bại!");
            return result;
        }





        public async Task<Request> GetPendingFoodOrderRequest(int customerId)
        {
            return await _context.Requests
                .Include(r => r.Customer)
                .Include(r => r.RequestStatus)
                .Include(r => r.Table)
                .Include(r => r.OrderDetails)
                    .ThenInclude(od => od.Item)
               .Where(r => r.CustomerId == customerId && r.RequestTypeId == 1 && r.RequestStatusId == 1)
               .FirstOrDefaultAsync();
        }

        public async Task<Request> GetLatestAcceptedRequest(int customerId)
        {
            return await _context.Requests
                .Where(r => r.CustomerId == customerId && r.RequestTypeId == 1 && r.RequestStatusId == 3)
                .OrderByDescending(r => r.CreatedAt)
                .FirstOrDefaultAsync();
        }

        public async Task<List<Request>> GetAcceptedOrdersAsync(string filter = "Accepted")
        {
            var query = _context.Requests
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
    }

}