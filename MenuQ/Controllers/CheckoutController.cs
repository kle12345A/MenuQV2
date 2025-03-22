using BussinessObject.invoice;
using BussinessObject.request;
using BussinessObject.vnpay;
using DataAccess.Enum;
using DataAccess.Models;
using MenuQ.Hubs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Identity.Client;

namespace MenuQ.Controllers
{
    [Route("Checkout")]
    public class CheckoutController : Controller
    {
        private readonly IVnPayService _vnPayService;
        private readonly MenuQContext _context;
        private readonly IInvoiceService _invoiceService;
        private readonly IRequestService _requestService;

        private readonly IHubContext<ServerHub> _hub;
        public CheckoutController(IVnPayService vnPayService, IInvoiceService invoiceService, IRequestService requestService, MenuQContext context, IHubContext<ServerHub> hub)
        {
            _invoiceService = invoiceService;
            _requestService = requestService;
            _hub = hub;
            _vnPayService = vnPayService;
            _context = context;
        }


        [Route("PaymentCallbackVnpay")]
        [HttpGet]
        public async Task<IActionResult> PaymentCallbackVnpay()
        {
            var response = _vnPayService.PaymentExecute(Request.Query);

            if (response.VnPayResponseCode == "00") // Giao dịch thành công
            {
                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        var newVnpayTransaction = new VnPayTransaction
                        {
                            OrderId = response.OrderId,
                            PaymentMethod = response.PaymentMethod,
                            OrderDescription = response.OrderDescription,
                            TransactionId = response.TransactionId,
                            PaymentId = response.PaymentId,
                            DateCreated = DateTime.Now
                        };
                        _context.Add(newVnpayTransaction);
                        await _context.SaveChangesAsync();

                        // 🔹 **Cập nhật lại request checkout**
                        var checkoutRequest = await _requestService.GetByIdAsync(int.Parse(response.OrderId));
                        if (checkoutRequest != null)
                        {
                            var updateRequest = await _requestService.AcceptRequest(checkoutRequest.RequestId);
                        }

                        var foodOrderRequest = await _requestService.GetPendingFoodOrderRequest((int)checkoutRequest.CustomerId);
                        if (foodOrderRequest == null)
                        {
                            throw new Exception("Không tìm thấy yêu cầu đặt món để cập nhật hóa đơn.");
                        }

                        var invoice = await _invoiceService.GetInvoiceByRequestId(foodOrderRequest.RequestId);
                        if (invoice != null)
                        {
                            var updateInvoice = await _invoiceService.CheckoutVnPay(invoice.InvoiceId);
                            if (!updateInvoice.Success)
                            {
                                throw new Exception("Không thể cập nhật trạng thái hóa đơn.");
                            }
                        }

                        await transaction.CommitAsync();

                        TempData["SuccessMessage"] = "Thanh toán thành công!";
                        _hub.Clients.All.SendAsync("LoadRequest");
                        return RedirectToAction("Index", "HomeApp");
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        TempData["ErrorMessage"] = "Đã xảy ra lỗi khi xử lý thanh toán.";
                        return RedirectToAction("PayOrder");
                    }
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Giao dịch VnPay thất bại.";
                return RedirectToAction("PayOrder");
            }
        }


    }
}
