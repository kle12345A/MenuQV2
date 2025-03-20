using System.Net.Http;
using DataAccess.Models.VnPay;
using Microsoft.AspNetCore.Http;

namespace BussinessObject.vnpay
{
    public interface IVnPayService
    {
        string CreatePaymentUrl(PaymentInformationModel model, HttpContext context);
        PaymentResponseModel PaymentExecute(IQueryCollection collections);

    }
}
