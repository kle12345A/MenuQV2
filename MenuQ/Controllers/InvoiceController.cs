
using BussinessObject.invoice;
using Microsoft.AspNetCore.Mvc;

namespace MenuQ.Controllers
{
    public class InvoiceController : Controller
    {
        private readonly IInvoiceService _invoiceService;

        public InvoiceController(IInvoiceService invoiceService)
        {
            _invoiceService = invoiceService;
        }

        public async Task<IActionResult> Index()
        {
            var invoices = await _invoiceService.GetAllAsync();
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
    }

}
