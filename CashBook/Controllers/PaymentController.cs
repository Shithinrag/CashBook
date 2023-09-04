using CashBook.ViewModels;
using DataAccessLayer.Abstraction;
using DataAccessLayer.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CashBook.Controllers
{
    public class PaymentController : Controller
    {
        
        private readonly IPaymentRepository _paymentRepository;
        public PaymentController(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }
        public IActionResult Index()
        {
            ViewBag.Options = new List<SelectListItem>
                            {
                                new SelectListItem { Value = "false", Text = "Active" },
                                new SelectListItem { Value = "true", Text = "Inactive" }
            };
            IEnumerable<Payment> paymentDetails = _paymentRepository.GetAllPayment();
            return PartialView("Payment/_paymentTable", paymentDetails);
        }        
        [HttpPost]         
        public IActionResult Create([FromBody] Payment payment)
        {  
           
            if (_paymentRepository.CheckDuplicatePayment(payment.Name))
            {
                _paymentRepository.AddPayment(payment);
                return Json(new { success = true, message = "Payment method added successfully" });
            }                        
            return Json(new { success = false, message = "ModelState failed or Name already exists" });
        }
        public IActionResult Update(int id)
        {
            Payment details = _paymentRepository.GetPaymentnDetails(id);           
            return Json(details);
        }
        [HttpPost]
        public IActionResult Update([FromBody] Payment payment)
        {
            if (_paymentRepository.CheckDuplicatePayment(payment.Name, payment.Id))
            {
                _paymentRepository.UpdatePayment(payment);
                return Json(new { success = true, message = "Payment method updated successfully" });
            }
            return Json(new { success = false, message = "Name already exists" });
        }
        public void ToggleStatus(int id)
        {
            _paymentRepository.ToggleStatus(id);
        }        
    }
}
