using DataAccessLayer.Entities;

namespace DataAccessLayer.Abstraction
{
    public interface ITransactionRepository
    {
        public void AddNonContraTransaction(Transaction transaction);
        public void AddContraTransaction(Transaction transaction);
        public void UpdateTransaction(Transaction transaction);
        public void DeleteTransaction(int id);
        public AllTransactions GetTransactionDetails(int id);
        public IEnumerable<AllTransactions> GetAllTransaction(string type);
    }
}
