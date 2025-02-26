using Microsoft.AspNetCore.Mvc;
using DataAccess.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Security.Cryptography;
using System.Text;

namespace MenuQ.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ResetPasswordController : Controller
    {
        private readonly MenuQContext _context;

        public ResetPasswordController(MenuQContext context)
        {
            _context = context;
        }

        public IActionResult Index(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Index", "ForgotPassword", new { area = "Admin" });
            }

            ViewBag.Email = email;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Submit(string email, string newPassword, string confirmPassword)
        {
            if (string.IsNullOrEmpty(email) || newPassword != confirmPassword)
            {
                ViewBag.Message = "Passwords do not match or email is invalid!";
                return View("Index");
            }

            var account = _context.Accounts.FirstOrDefault(a => a.Email == email);
            if (account == null)
            {
                ViewBag.Message = "Account not found!";
                return View("Index");
            }

            // Hash mật khẩu mới với salt
            account.Password = HashPassword(newPassword);

            _context.Accounts.Update(account);
            await _context.SaveChangesAsync();

            ViewBag.Message = "Password has been reset successfully!";
            return RedirectToAction("Index", "Auth", new { area = "Admin" });
        }

        private string HashPassword(string password)
        {
            byte[] salt = new byte[16]; // Salt 16 byte
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            byte[] hashed = KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 32 // 256-bit key
            );

            // Ghép salt và mật khẩu băm lại thành một chuỗi để lưu vào DB
            return Convert.ToBase64String(salt) + ":" + Convert.ToBase64String(hashed);
        }

        private bool VerifyPassword(string password, string storedHash)
        {
            var parts = storedHash.Split(':');
            if (parts.Length != 2)
            {
                return false; // Định dạng mật khẩu lưu không hợp lệ
            }

            byte[] salt = Convert.FromBase64String(parts[0]);
            byte[] storedPasswordHash = Convert.FromBase64String(parts[1]);

            byte[] computedHash = KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 32 // 256-bit key
            );

            return storedPasswordHash.SequenceEqual(computedHash);
        }
    }
}
