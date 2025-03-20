using BussinessObject.vnpay;
using DataAccess.Models;
using Microsoft.AspNetCore.Mvc;

namespace MenuQ.Controllers
{
   [Route("Checkout")]
public class CheckoutController : Controller
{
    private readonly IVnPayService _vnPayService;
    private readonly MenuQContext _context;
    public CheckoutController(IVnPayService vnPayService, MenuQContext context)
    {

        _vnPayService = vnPayService;
        _context = context;
    }
    [Route("PaymentCallbackVnpay")]
    [HttpGet]
    public async Task<IActionResult> PaymentCallbackVnpay()
    {
        var response = _vnPayService.PaymentExecute(Request.Query);
        if (response.VnPayResponseCode == "00") //giao dịch thành công lưu db
        {
            var newVnpayInsert = new VnPayTransaction
            {
                OrderId = response.OrderId,
                PaymentMethod = response.PaymentMethod,
                OrderDescription = response.OrderDescription,
                TransactionId = response.TransactionId,
                PaymentId = response.PaymentId,
                DateCreated = DateTime.Now
            };  
            _context.Add(newVnpayInsert);
            await _context.SaveChangesAsync();
            // Tiến hành đặt đơn hàng khi thanh toán momo thành công 
            //await Checkout();
            return View("PaymentCallbackVnpay", response);
        }
        else
        {
            TempData["error"] = "Giao dịch VnPay thất bại.";
            return RedirectToPage("/Payment");
            //return Json(response); 7
        }
    }
}
}
