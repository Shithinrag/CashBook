using CashBook.ViewModels;
using DataAccessLayer.Abstraction;
using DataAccessLayer.Entities;
using Microsoft.AspNetCore.Mvc;


namespace CashBook.Components
{
    public class GetAllTransactionViewComponent : ViewComponent
    {
        private readonly ITransactionRepository _transactionRepository;

        public GetAllTransactionViewComponent(ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;
        }
        public async Task<IViewComponentResult> InvokeAsync(string type)
        {
            IEnumerable<AllTransactions> transactions = _transactionRepository.GetAllTransaction(type);
            //ViewData[$"{type}"]= transactions.Count();
            List<TransactionViewModel> transactionViewModels = new();                        
            foreach(var item in transactions)
            {
                var vm = new TransactionViewModel
                {
                    Id = item.Id,
                    Date = item.Date.ToString("dd/MM/yyyy"),
                    Amount = item.Amount,
                    Description = item.Description,
                    CategoryIdExpense = item.CategoryId,
                    CategoryNameExpense = item.CategoryName,
                    PaymentFromIdSingle = item.PaymentFromId,
                    PaymentFromNameSingle = item.PaymentFromName,
                    PaymentToId = item.PaymentToId,
                    PaymentToName = item.PaymentToName,
                    Type = item.Type
                };
                transactionViewModels.Add(vm);
            }
            return View("GetAllTransaction",transactionViewModels);
        }
    }
}
