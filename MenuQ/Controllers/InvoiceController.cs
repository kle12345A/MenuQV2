﻿
using BussinessObject.invoice;
using BussinessObject.request;
using DataAccess.Enum;
using DataAccess.Models;
using MenuQ.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Net.WebSockets;

namespace MenuQ.Controllers
{
    [Authorize(Roles = "Admin,Employee")]
    public class InvoiceController : Controller
    {
        private readonly IInvoiceService _invoiceService;
        private readonly IRequestService _requestService;
        private readonly IHubContext<ServerHub> _hub;

        public InvoiceController(IInvoiceService invoiceService, IRequestService requestService, IHubContext<ServerHub> hub)
        {
            _invoiceService = invoiceService;
            _requestService = requestService;
            _hub = hub;
        }

        public async Task<IActionResult> Index()
        {
            var invoices = await _invoiceService.GetAllInvoiceAsync();
            return View(invoices);
        }

        public async Task<IActionResult> Details(int requestId)
        {
            var invoice = await _invoiceService.GetInvoiceByRequestId(requestId);
            if (invoice == null)
            {
                TempData["ErrorMessage"] = "Invoice not found for this request.";
                return RedirectToAction("Index");
            }

            return View(invoice);
        }

        [HttpGet]
        public async Task<IActionResult> PaymentDetails(int customerId, string paymentMethod)
        {
            var invoiceDto = await _invoiceService.GetInvoiceByCustomer(customerId);
            if (invoiceDto == null)
            {
                Console.WriteLine("Khong tim dc invoice ");
                TempData["ErrorMessage"] = "Khong tim thay invoice.";
                return RedirectToAction("Index");
            }

            var checkoutRequest = await _requestService.GetCheckoutRequest(customerId);
            if (checkoutRequest == null)
            {
                Console.WriteLine("Khong tim thay request  ");
                TempData["ErrorMessage"] = "Không tìm thấy yêu cầu thanh toán.";
                return RedirectToAction("Index");
            }

            var processPayment = await _requestService.ProcessRequest(checkoutRequest.RequestId);
            if (invoiceDto == null)
            {
                return NotFound("Không tìm thấy hóa đơn cần thanh toán ở payment detail.");
            }

            //Chuyển đổi InvoiceStatus từ string về int để so sánh đúng kiểu dữ liệu
            if (Enum.TryParse<InvoiceStatus>(invoiceDto.InvoiceStatus, out InvoiceStatus status)
                && status == InvoiceStatus.Serving)
            {
                var updateResult = await _invoiceService.UpdateInvoiceStatus(invoiceDto.InvoiceId, InvoiceStatus.ProcessingPayment);
                if (!updateResult.Success)
                {
                    TempData["ErrorMessage"] = "Không thể cập nhật trạng thái hóa đơn.";
                    return RedirectToAction("Index");
                }
            }


            //Cập nhật phương thức thanh toán
            bool updateSuccess = await _invoiceService.UpdatePaymentMethod(invoiceDto.InvoiceId, paymentMethod);
            if (!updateSuccess)
            {
                TempData["ErrorMessage"] = "Không thể cập nhật phương thức thanh toán.";
            }

            invoiceDto.PaymentMethod = paymentMethod;

            return View("PaymentDetails", invoiceDto);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmPayment(int invoiceId, int customerId)
        {
            var result = await _invoiceService.Checkout(invoiceId);
            if (!result.Success)
            {
                TempData["ErrorMessage"] = result.Message;
                return RedirectToAction("PaymentDetails", new { invoiceId, customerId });
            }

            var checkoutRequest = await _requestService.GetCheckoutRequest(customerId);
            if (checkoutRequest != null)
            {
                int? accountId = HttpContext.Session.GetInt32("Acc");
                if (accountId == null)
                {
                    TempData["Message"] = "Bạn cần đăng nhập.";
                    return RedirectToAction("Index", "Auth", new { area = "Admin" });
                }
                var updateRequest = await _requestService.AcceptRequest(checkoutRequest.RequestId, accountId.Value);
                if (!updateRequest.Success)
                {
                    TempData["ErrorMessage"] = "Không thể cập nhật trạng thái yêu cầu thanh toán.";
                }
            }

            TempData["SuccessMessage"] = "Thanh toán thành công!";
            _hub.Clients.All.SendAsync("LoadRequest");
            return RedirectToAction("Index", "Requests");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelPayment(int invoiceId, int customerId)
        {
            var result = await _invoiceService.CancelCheckout(invoiceId);
            if (!result.Success)
            {
                TempData["ErrorMessage"] = result.Message;
                return RedirectToAction("PaymentDetails", new { invoiceId, customerId });
            }

            var checkoutRequest = await _requestService.GetCheckoutRequest(customerId);
            if (checkoutRequest != null)
            {
                int? accountId = HttpContext.Session.GetInt32("Acc");
                if (accountId == null)
                {
                    TempData["Message"] = "Bạn cần đăng nhập.";
                    return RedirectToAction("Index", "Auth", new { area = "Admin" });
                }
                var updateRequest = await _requestService.RejectRequest(checkoutRequest.RequestId, 1, accountId.Value);
                if (!updateRequest.Success)
                {
                    TempData["ErrorMessage"] = "Không thể cập nhật trạng thái yêu cầu thanh toán.";
                }
            }
            _hub.Clients.All.SendAsync("LoadRequest");
            return RedirectToAction("Index", "Requests");
        }

    }

}
