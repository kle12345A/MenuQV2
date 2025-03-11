
using BussinessObject.invoice;
using BussinessObject.request;
using DataAccess.Enum;
using DataAccess.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;

namespace MenuQ.Controllers
{
    public class InvoiceController : Controller
    {
        private readonly IInvoiceService _invoiceService;
        private readonly IRequestService _requestService;

        public InvoiceController(IInvoiceService invoiceService, IRequestService requestService)
        {
            _invoiceService = invoiceService;
            _requestService = requestService;
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


        [HttpGet]
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
                var updateRequest = await _requestService.AcceptRequest(checkoutRequest.RequestId);
                if (!updateRequest.Success)
                {
                    TempData["ErrorMessage"] = "Không thể cập nhật trạng thái yêu cầu thanh toán.";
                }
            }

            TempData["SuccessMessage"] = "Thanh toán thành công!";
            return RedirectToAction("Index", "Invoice");
        }

        [HttpPost]
        public async Task<IActionResult> CancelPayment(int invoiceId, int customerId)
        {
            var invoiceDto = await _invoiceService.GetInvoiceByCustomer(customerId);
            if (invoiceDto == null)
            {
                return NotFound("Không tìm thấy hóa đơn.");
            }

            if (int.TryParse(invoiceDto.InvoiceStatus, out int status) && status == (int)InvoiceStatus.ProcessingPayment)
            {
                var resetResult = await _invoiceService.UpdateInvoiceStatus(invoiceDto.InvoiceId, InvoiceStatus.Serving);
                if (!resetResult.Success)
                {
                    TempData["ErrorMessage"] = "Không thể đặt lại trạng thái hóa đơn.";
                    return RedirectToAction("Index");
                }

                await _invoiceService.ResetPaymentMethod(invoiceDto.InvoiceId);

                var checkoutRequest = await _requestService.GetCheckoutRequest(customerId);
                if (checkoutRequest != null)
                {
                    var updateRequest = await _requestService.RejectRequest(checkoutRequest.RequestId, 3);
                    if (!updateRequest.Success)
                    {
                        TempData["ErrorMessage"] = "Không thể cập nhật trạng thái yêu cầu thanh toán.";
                    }
                }
            }

            return RedirectToAction("Index", "Invoice");
        }
    }

}
