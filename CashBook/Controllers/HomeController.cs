using DataAccessLayer;
using DataAccessLayer.Abstraction;
using DataAccessLayer.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Diagnostics;

namespace CashBook.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ITransactionRepository _transactionRepository;

        public HomeController(ILogger<HomeController> logger, ITransactionRepository transactionRepository)
        {
            _logger = logger;
            _transactionRepository = transactionRepository;
            
        }
        public IActionResult Index()
        {
            ViewBag.Options = new List<SelectListItem>
                            {
                                new SelectListItem { Value = "false", Text = "Active" },
                                new SelectListItem { Value = "true", Text = "Inactive" }
            };
            //IEnumerable<Transaction> transactions= _transactionRepository.GetAllTransaction();
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}