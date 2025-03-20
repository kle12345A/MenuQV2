using BussinessObject.vnpay;
using DataAccess.Models.VnPay;
using Microsoft.AspNetCore.Mvc;

namespace MenuQ.Controllers
{
    [Route("Payment")]
    public class PaymentController : Controller
    {
        private readonly IVnPayService _vnPayService;
        public PaymentController(IVnPayService vnPayService)
        {

            _vnPayService = vnPayService;
        }
        [Route("CreatePaymentUrlVnpay")]
        [HttpPost, HttpGet]
        public IActionResult CreatePaymentUrlVnpay(PaymentInformationModel model)
        {
            var url = _vnPayService.CreatePaymentUrl(model, HttpContext);
            return Redirect(url);
        }

    }
}
