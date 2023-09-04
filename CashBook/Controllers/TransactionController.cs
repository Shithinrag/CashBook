
using CashBook.ViewModels;
using DataAccessLayer.Abstraction;
using DataAccessLayer.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CashBook.Controllers
{
    public class TransactionController : Controller
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IPaymentRepository _paymentRepository;
        public TransactionController(ITransactionRepository transactionRepository, ICategoryRepository categoryRepository, IPaymentRepository paymentRepository)
        {
            _transactionRepository = transactionRepository;
            _categoryRepository = categoryRepository;
            _paymentRepository = paymentRepository;
        }

        public ActionResult Index()
        {
            ViewBag.Options = new List<SelectListItem>
                            {
                                new SelectListItem { Value = "false", Text = "Active" },
                                new SelectListItem { Value = "true", Text = "Inactive" }
            };
            return View();
        }
        public ActionResult Details(int id)
        {
            AllTransactions allTransactions = _transactionRepository.GetTransactionDetails(id);
            TransactionViewModel transactionViewModel = new()
            {
                    Id = allTransactions.Id,
                    Date = allTransactions.Date.ToString("dd/MM/yyyy"),
                    Amount = allTransactions.Amount,
                    Description = allTransactions.Description,
                    CategoryIdExpense = allTransactions.CategoryId,
                    CategoryNameExpense = allTransactions.CategoryName,
                    PaymentFromIdSingle = allTransactions.PaymentFromId,
                    PaymentFromNameSingle = allTransactions.PaymentFromName,
                    PaymentToId = allTransactions.PaymentToId,
                    PaymentToName = allTransactions.PaymentToName,
                    Type = allTransactions.Type
            };
            return Json(transactionViewModel);            
        }
        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.Options = new List<SelectListItem>
                            {
                                new SelectListItem { Value = "false", Text = "Active" },
                                new SelectListItem { Value = "true", Text = "Inactive" }
            };
            ViewBag.Type = new List<SelectListItem>
                            {
                                new SelectListItem { Value = "Expense", Text = "Expense" },
                                new SelectListItem { Value = "Income", Text = "Income" },
                                new SelectListItem { Value = "Contra", Text = "Contra" }
                                //new SelectListItem { Value = "Multiple", Text = "Multiple" }
            };
            ViewBag.ExpenseCategories = new SelectList(_categoryRepository.GetActiveCategory("Expense"), "Id", "Name");
            ViewBag.IncomeCategories = new SelectList(_categoryRepository.GetActiveCategory("Income"), "Id", "Name");
            ViewBag.PaymentList = new SelectList(_paymentRepository.GetActivePayments(), "Id", "Name");

            //_transactionRepository.AddTransaction(transaction);
            return View();
        }
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create(TransactionViewModel transactionViewModel)
        {
            if (ModelState.IsValid)
            {
                switch (transactionViewModel.Type)
                {
                    case "Expense":
                        Transaction transaction = new()
                        {
                            Date = Convert.ToDateTime(transactionViewModel.Date),
                            Description = transactionViewModel.Description,
                            CategoryId = transactionViewModel.CategoryIdExpense,
                            PaymentFromId = transactionViewModel.PaymentFromIdSingle,
                            CreatedDate = DateTime.Now,
                            Amount = transactionViewModel.Amount,
                            Type = transactionViewModel.Type
                        };
                        _transactionRepository.AddNonContraTransaction(transaction);
                        break;
                    case "Income":
                        Transaction transaction2 = new()
                        {
                            Date = Convert.ToDateTime(transactionViewModel.Date),
                            Description = transactionViewModel.Description,
                            CategoryId = transactionViewModel.CategoryIdIncome,
                            PaymentFromId = transactionViewModel.PaymentFromIdSingle,
                            CreatedDate = DateTime.Now,
                            Amount = transactionViewModel.Amount,
                            Type = transactionViewModel.Type
                        };
                        _transactionRepository.AddNonContraTransaction(transaction2);
                        break;
                    case "Contra":
                        Transaction transaction3 = new()
                        {
                            Date = Convert.ToDateTime(transactionViewModel.Date),
                            Description = transactionViewModel.Description,
                            PaymentFromId = transactionViewModel.PaymentFromIdContra,
                            PaymentToId = transactionViewModel.PaymentToId,
                            CreatedDate = DateTime.Now,
                            Amount = transactionViewModel.Amount,
                            Type = transactionViewModel.Type
                        };
                        _transactionRepository.AddContraTransaction(transaction3);
                        break;
                    case "Multi":
                        Transaction transaction4 = new()
                        {
                            Date = Convert.ToDateTime(transactionViewModel.Date),
                            Description = transactionViewModel.Description,
                            PaymentFromId = transactionViewModel.PaymentFromIdContra,
                            PaymentToId = transactionViewModel.PaymentToId,
                            CreatedDate = DateTime.Now,
                            Amount = transactionViewModel.Amount,
                            Type = transactionViewModel.Type
                        };
                        //Not implemented yet
                        break;
                    default:
                        break;
                }
                ViewBag.Options = new List<SelectListItem>
                            {
                                new SelectListItem { Value = "false", Text = "Active" },
                                new SelectListItem { Value = "true", Text = "Inactive" }
            };
                ViewBag.Type = new List<SelectListItem>
            {
                new SelectListItem { Value = "Expense", Text = "Expense"},
                new SelectListItem { Value = "Income", Text = "Income" },
                new SelectListItem { Value = "Contra", Text = "Contra" }
            };
                ViewBag.ExpenseCategories = new SelectList(_categoryRepository.GetActiveCategory("Expense"), "Id", "Name");
                ViewBag.IncomeCategories = new SelectList(_categoryRepository.GetActiveCategory("Income"), "Id", "Name");
                ViewBag.PaymentList = new SelectList(_paymentRepository.GetActivePayments(), "Id", "Name");
                return View();
            }
            ViewBag.Options = new List<SelectListItem>
                            {
                                new SelectListItem { Value = "false", Text = "Active" },
                                new SelectListItem { Value = "true", Text = "Inactive" }
            };
            ViewBag.Type = new List<SelectListItem>
            {
                new SelectListItem { Value = "Expense", Text = "Expense"},
                new SelectListItem { Value = "Income", Text = "Income" },
                new SelectListItem { Value = "Contra", Text = "Contra" }
            };
            ViewBag.ExpenseCategories = new SelectList(_categoryRepository.GetActiveCategory("Expense"), "Id", "Name");
            ViewBag.IncomeCategories = new SelectList(_categoryRepository.GetActiveCategory("Income"), "Id", "Name");
            ViewBag.PaymentList = new SelectList(_paymentRepository.GetActivePayments(), "Id", "Name");
            return View(transactionViewModel);
        }

        public ActionResult Edit(int id)
        {           
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: HomeController1/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: HomeController1/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
        
    }
}
