using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using BussinessObject.email;
using System;
using System.Linq;
using System.Threading.Tasks;
using DataAccess.Models;

namespace MenuQ.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ForgotPasswordController : Controller
    {
        private readonly IEmailService _emailService;
        private readonly MenuQContext _context;

        public ForgotPasswordController(IEmailService emailService, MenuQContext context)
        {
            _emailService = emailService;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SendOtp(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                ViewBag.Message = "Please enter your email.";
                return View("Index");
            }

            var account = _context.Accounts.FirstOrDefault(a => a.Email == email);
            if (account == null)
            {
                ViewBag.Message = "No account found with this email.";
                return View("Index");
            }

            var otp = GenerateOtp();
            var otpTime = DateTime.UtcNow.AddSeconds(30);
            HttpContext.Session.SetString("Email", email);

            HttpContext.Session.SetString("Otp", otp);
            HttpContext.Session.SetString("OtpTime", otpTime.ToString("o"));

            await _emailService.SendEmailAsync(email, "Your OTP Code", $"Your OTP is: {otp}. It will expire in 30 seconds.");

            ViewBag.Message = "OTP has been sent to your email.";
            ViewBag.OtpSent = true;
            return View("Index");
        }

        [HttpPost]
        public IActionResult VerifyOtp(string email, string otp)
        {
            var savedEmail = HttpContext.Session.GetString("Email");
            if (string.IsNullOrEmpty(savedEmail) || savedEmail != email)
            {
                ViewBag.Message = "Session expired or email mismatch.";
                return View("Index");
            }

            var savedOtp = HttpContext.Session.GetString("Otp");
            var savedOtpTime = HttpContext.Session.GetString("OtpTime");

            if (string.IsNullOrEmpty(savedOtp) || DateTime.UtcNow > DateTime.Parse(savedOtpTime))
            {
                ViewBag.Message = "OTP has expired or was not generated.";
                return View("Index");
            }

            if (otp != savedOtp)
            {
                ViewBag.Message = "Invalid OTP. Please try again.";
                return View("Index");
            }

            return RedirectToAction("Index", "ResetPassword", new { area = "Admin", email = savedEmail });
        }




        private string GenerateOtp(int length = 6)
        {
            var random = new Random();
            var otp = new char[length];
            for (int i = 0; i < length; i++)
            {
                otp[i] = (char)('0' + random.Next(0, 10));
            }
            return new string(otp);
        }
    }
}
