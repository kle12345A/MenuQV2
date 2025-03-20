
using DataAccess.Libraries;
using BussinessObject.vnpay;
using DataAccess.Models.VnPay;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace BussinessObject.vnpay;
public class VnPayService : IVnPayService
{
    private readonly IConfiguration _configuration;
    public VnPayService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string CreatePaymentUrl(PaymentInformationModel model, HttpContext context)
    {
        try
        {
            var timeZoneById = TimeZoneInfo.FindSystemTimeZoneById(_configuration["TimeZoneId"]);
            var timeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneById);
            var tick = timeNow.ToString("yyyyMMddHHmmss");
            var pay = new VnPayLibrary();
            var urlCallBack = _configuration["Vnpay:PaymentBackReturnUrl"];

            pay.AddRequestData("vnp_Version", _configuration["Vnpay:Version"]);
            pay.AddRequestData("vnp_Command", _configuration["Vnpay:Command"]);
            pay.AddRequestData("vnp_TmnCode", _configuration["Vnpay:TmnCode"]);
            pay.AddRequestData("vnp_Amount", (Convert.ToInt32(model.Amount) * 100).ToString());
            pay.AddRequestData("vnp_CreateDate", timeNow.ToString("yyyyMMddHHmmss"));
            pay.AddRequestData("vnp_CurrCode", _configuration["Vnpay:CurrCode"]);
            pay.AddRequestData("vnp_IpAddr", pay.GetIpAddress(context));
            pay.AddRequestData("vnp_Locale", _configuration["Vnpay:Locale"]);
            pay.AddRequestData("vnp_OrderInfo", $"{model.Name} {model.OrderDescription} {model.Amount}");
            pay.AddRequestData("vnp_OrderType", string.IsNullOrEmpty(model.OrderType) ? "other" : model.OrderType);
            pay.AddRequestData("vnp_ReturnUrl", urlCallBack);
            pay.AddRequestData("vnp_TxnRef", tick);

            var paymentUrl = pay.CreateRequestUrl(_configuration["Vnpay:BaseUrl"], _configuration["Vnpay:HashSecret"]);
            return paymentUrl;
        }
        catch (Exception ex)
        {
            // Log lỗi (nếu bạn có logger)
            Console.WriteLine($"Error in CreatePaymentUrl: {ex.Message}");
            return null;
        }
    }
    public PaymentResponseModel PaymentExecute(IQueryCollection collections)
    {
        var pay = new VnPayLibrary();
        var response = pay.GetFullResponseData(collections, _configuration["Vnpay:HashSecret"]);
        return response;
    }

}

