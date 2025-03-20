using BussinessObject.account;
using DataAccess.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using System.Text.Json;
using MenuQ.Models;
using System.Net;

namespace MenuQ.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AuthController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly EmailSettings _emailSettings;

        public AuthController(IAccountService accountService, IOptions<EmailSettings> emailSettings)
        {
            _accountService = accountService;
            _emailSettings = emailSettings.Value;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Register()
        {
            return View(new RegisterViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Hiển thị các lỗi validation chi tiết
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    ViewBag.Message += error.ErrorMessage + " ";
                }
                return View(model);
            }

            // Kiểm tra tài khoản đã tồn tại chưa
            if (await _accountService.IsAccountExists(model.Username, model.Email))
            {
                ViewBag.Message = "Username or email already exists.";
                return View(model);
            }

            // Tạo OTP và lưu vào Session
            string otp = GenerateOTP();
            HttpContext.Session.SetString("OTP", otp);
            HttpContext.Session.SetObject("PendingAccount", model);

            // Gửi email OTP
            try
            {
                await SendOTPEmail(model.Email, otp);
                return RedirectToAction("VerifyOTP");
            }
            catch (Exception ex)
            {
                ViewBag.Message = $"Failed to send OTP: {ex.Message}";
                return View(model);
            }
        }

        public IActionResult VerifyOTP()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> VerifyOTP(string otp)
        {
            var storedOTP = HttpContext.Session.GetString("OTP");
            var pendingAccount = HttpContext.Session.GetObject<RegisterViewModel>("PendingAccount");

            if (pendingAccount == null || storedOTP == null || otp != storedOTP)
            {
                ViewBag.Message = "Invalid OTP or session expired.";
                return View();
            }

            // Tạo Account mới
            var newAccount = new Account
            {
                UserName = pendingAccount.Username,
                Email = pendingAccount.Email,
                Password = pendingAccount.Password, 
                PhoneNumber = pendingAccount.PhoneNumber,
                RoleId = 2,
                Active = true,
                CreatedAt = DateTime.UtcNow
            };

            // Lưu vào database
            var result = await _accountService.AddAsync(newAccount);
            if (result == 1)
            {
                HttpContext.Session.Remove("OTP");
                HttpContext.Session.Remove("PendingAccount");

                
                ViewBag.Message = "Registration successful! Please log in.";
                return RedirectToAction("Index");
            }

            ViewBag.Message = "Registration failed.";
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            if (!ModelState.IsValid)
            {
                return View("Index");
            }

            var account = await _accountService.LoginAsync(username, password);
            if (account == null)
            {
                ViewBag.Message = "Username or password incorrect.";
                return View("Index");
            }

            if ((bool)!account.Active)
            {
                ViewBag.Message = "Account is disabled";
                return View("Index");
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, account.UserName),
                new Claim(ClaimTypes.Email, account.Email),
                new Claim(ClaimTypes.Role, account.Role.RoleName)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = false,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(2)
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

            HttpContext.Session.SetString("UserName", account.UserName);
            HttpContext.Session.SetInt32("Acc", account.AccountId);
            HttpContext.Session.SetString("Email", account.Email);

            // Kiểm tra vai trò và chuyển hướng
            if (account.Role.RoleName == "Admin")
            {
                return RedirectToAction("", "Dashboard");
            }
            else if (account.Role.RoleName == "Employee")
            {
                return RedirectToAction("Index", "Requests", new { area = "" });
            }
            else
            {
                return RedirectToAction("Index", "Dashboard");
            }
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

        // Helper methods
        private string GenerateOTP()
        {
            Random random = new Random();
            return random.Next(100000, 999999).ToString();
        }

        private async Task SendOTPEmail(string email, string otp)
        {
            var fromAddress = new MailAddress(_emailSettings.FromEmail, "MenuQ");
            var toAddress = new MailAddress(email);
            const string subject = "Your OTP for MenuQ Registration";
            string body = $"Your OTP is: {otp}. It will expire in 5 minutes.";

            var smtp = new SmtpClient
            {
                Host = _emailSettings.SmtpServer,
                Port = _emailSettings.Port,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_emailSettings.FromEmail, _emailSettings.Password)
            };

            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body
            })
            {
                await smtp.SendMailAsync(message);
            }
        }
    }

    // Extension methods cho Session
    public static class SessionExtensions
    {
        public static void SetObject<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonSerializer.Serialize(value));
        }

        public static T GetObject<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default : JsonSerializer.Deserialize<T>(value);
        }
    }
}