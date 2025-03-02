using BussinessObject.cancellreason;
using BussinessObject.request;
using DataAccess.Models;
using DataAccess.Repository.cancellation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;

namespace MenuQ.Controllers
{
    public class ResetRequestDto
    {
        public int Id { get; set; }
    }


    public class RequestsController : Controller
    {
        private readonly IRequestService _requestService;
        private readonly ICancellReasonService _cancellationService;
        private readonly ILogger<RequestsController> _logger;

        public RequestsController(IRequestService requestService, ICancellReasonService cancellationService, ILogger<RequestsController> logger)
        {
            _requestService = requestService;
            _cancellationService = cancellationService;
            _logger = logger;
        }

        // Hiển thị danh sách yêu cầu
        public async Task<IActionResult> Index(string type = "All")
        {
            //var pendingRequests = await _requestService.GetPendingRequests(type);
            var pendingRequests = await _requestService.GetAllRequestsWithNotes();
            var cancellationReasons = await _cancellationService.GetActiveCancellationReasons();

            ViewBag.CancellationReasons = new SelectList(cancellationReasons, "ReasonId", "ReasonText");

            return View(pendingRequests);
        }

        // Hiển thị chi tiết yêu cầu
        public async Task<IActionResult> Details(int id)
        {
            //Kiểm tra Request có tồn tại không
            var request = await _requestService.GetRequestDetailsAsync(id);
            if (request == null)
            {
                return NotFound();
            }

            //Nếu là "Food Order" (RequestTypeId = 1) và trạng thái là Pending (1), cập nhật sang "InProcess" (2)
            //if (request.RequestTypeId == 1 && request.RequestStatusId == 1 )
            if (request.RequestTypeId == 1 || request.RequestTypeId == 3)
            {
                var result = await _requestService.ProcessRequest(id);
                if (result.Success)
                {
                    //Lấy lại request mới nhất từ database sau khi cập nhật
                    request = await _requestService.GetRequestDetailsAsync(id);
                }
                else
                {
                    TempData["ErrorMessage"] = result.Message; // Nếu lỗi, hiển thị thông báo
                }
            }

            //Lấy danh sách lý do hủy
            var cancellationReasons = await _cancellationService.GetActiveCancellationReasons();
            ViewBag.CancellationReasons = new SelectList(cancellationReasons, "ReasonId", "ReasonText");

            return View(request);
        }


        public async Task<IActionResult> RequestHistory()
        {
            var requests = await _requestService.GetAllRequestsAsync();
            return View(requests);
        }

        // Chấp nhận yêu cầu (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Accept(int id)
        {
            var result = await _requestService.AcceptRequest(id);
            TempData["Message"] = result.Message;
            return RedirectToAction("Index");
        }

        // Từ chối yêu cầu (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id, int reasonId)
        {
            if (reasonId <= 0)
            {
                TempData["Message"] = "Invalid cancellation reason.";
                return RedirectToAction("Index");
            }

            _logger.LogInformation("Reject RequestID: {RequestId} with ReasonID: {ReasonId}", id, reasonId);
            var result = await _requestService.RejectRequest(id, reasonId);

            TempData["Message"] = result.Message;
            return RedirectToAction("Index");
        }


        // Cập nhật trạng thái yêu cầu (ví dụ: chuyển sang inprocess)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatusInprocess(int id, int? accountId = null)
        {
            var result = await _requestService.ProcessRequest(id);
            TempData["Message"] = result.Message;
            return RedirectToAction("Index");
        }

        // Xử lý thanh toán (checkout)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DirectToCheckoutPage(int requestId)
        {
            var result = await _requestService.AcceptRequest(requestId);
            TempData["Message"] = result.Message;

            if (result.Success)
            {
                //Chuyển hướng sang trang chi tiết hóa đơn với requestId
                return RedirectToAction("Details", "Invoice", new { requestId = requestId });
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> ResetStatus([FromBody] ResetRequestDto requestDto)
        {
            int requestId = requestDto.Id;

            _logger.LogInformation("API ResetStatus called for RequestID: {RequestId}", requestId);

            var result = await _requestService.ResetPendingRequest(requestId);

            if (!result.Success)
            {
                _logger.LogWarning("ResetStatus API failed for RequestID: {RequestId}. Message: {Message}", requestId, result.Message);
                return Json(new { success = false, message = result.Message });
            }

            _logger.LogInformation("ResetStatus API success for RequestID: {RequestId}");
            return Json(new { success = true, message = result.Message });
        }






    }
}