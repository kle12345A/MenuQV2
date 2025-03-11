using Microsoft.AspNetCore.Mvc;

namespace MenuQ.Controllers
{
    public class ErrorController : Controller
    {
        [Route("Errors/404")]
        public IActionResult NotFoundPage()
        {
            Response.StatusCode = 404; // Đặt mã trạng thái ở Controller
            return View("404"); // Trả về View 404.cshtml
        }
    }
}
