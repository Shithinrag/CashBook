using DataAccessLayer.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using DataAccessLayer.Abstraction;
using Logger;

namespace DataAccessLayer.Repositories
{
    public class TransactionRepository :ITransactionRepository
    {
        private readonly AppDb _appDb;        
        public TransactionRepository(AppDb appDb)
        {
            _appDb = appDb;
          
        }

        public void AddNonContraTransaction(Transaction transaction)
        {
            SqlParameter[] parameters = new SqlParameter[7];

            parameters[0] = new SqlParameter("@date",transaction.Date);
            parameters[1] = new SqlParameter("@desc", transaction.Description);
            parameters[2] = new SqlParameter("@cat", transaction.CategoryId);
            parameters[3] = new SqlParameter("@payFrom", transaction.PaymentFromId);           
            parameters[4] = new SqlParameter("@amt", transaction.Amount);
            parameters[5] = new SqlParameter("@createdDate", transaction.CreatedDate);            
            parameters[6] = new SqlParameter("@type", transaction.Type);
            try
            {
                _appDb.Database.ExecuteSqlRaw("Add_NonContra_Transaction @date,@desc,@cat,@payFrom,@amt,@createdDate,@type", parameters);
            }
            catch (Exception ex)
            {
                CashBookLogger.LogError(ex, "Inside TransactionRepository-AddNonContraTransaction");  
                Console.Error.WriteLine(ex.Message);
            }
        }
        public void AddContraTransaction(Transaction transaction)
        {
            SqlParameter[] parameters = new SqlParameter[7];

            parameters[0] = new SqlParameter("@date", transaction.Date);
            parameters[1] = new SqlParameter("@desc", transaction.Description);           
            parameters[2] = new SqlParameter("@payFrom", transaction.PaymentFromId);
            parameters[3] = new SqlParameter("@payTo", transaction.PaymentToId);
            parameters[4] = new SqlParameter("@amt", transaction.Amount);
            parameters[5] = new SqlParameter("@createdDate", transaction.CreatedDate);
            parameters[6] = new SqlParameter("@type", transaction.Type);
            try
            {
                _appDb.Database.ExecuteSqlRaw("Add_Contra_Transaction @date,@desc,@payFrom,@payTo,@amt,@createdDate,@type", parameters);
            }
            catch (Exception ex)
            {
                CashBookLogger.LogError(ex, "Inside TransactionRepository-AddContraTransaction");
                Console.Error.WriteLine(ex.Message);
            }
        }
        public void UpdateTransaction(Transaction transaction)
        {
            SqlParameter[] parameters = new SqlParameter[6];

            parameters[0] = new SqlParameter("@date", transaction.Date);
            parameters[1] = new SqlParameter("@desc", transaction.Description);
            parameters[2] = new SqlParameter("@cat", transaction.CategoryId);
            parameters[3] = new SqlParameter("@pay", transaction.PaymentFromId);
            parameters[4] = new SqlParameter("@createdDate", transaction.CreatedDate);
            parameters[5] = new SqlParameter("@id", transaction.Id);
            try
            {
                _appDb.Database.ExecuteSqlRaw("Add_Transaction @date,@desc,@cat,@pay,@createdDate,@id", parameters);
            } 
            catch (Exception ex)
            {
                CashBookLogger.LogError(ex, "Inside TransactionRepository");
                Console.Error.WriteLine(ex.Message);
            }

        }
        public void DeleteTransaction(int id)
        {
            List<SqlParameter> parameters = new ()
            {
                new SqlParameter("@id", id)
            };
            try
            {
                _appDb.Database.ExecuteSqlRaw("Delete_Transaction @id", parameters);
            }
            catch (Exception ex)
            {
                CashBookLogger.LogError(ex, "Inside TransactionRepository");
                Console.Error.WriteLine(ex.Message);
            }

        }
        public AllTransactions GetTransactionDetails(int id)
        {
            //List<SqlParameter> parameters = new()
            //{
            //    new SqlParameter("@id", id)
            //};
            SqlParameter[] parameters = new SqlParameter[1];
            parameters[0] = new SqlParameter("@id", id);
            try
            {
                var result = _appDb.AllTransactions.FromSqlRaw<AllTransactions>("GetDetails_Transaction @id", parameters).AsEnumerable().FirstOrDefault();
                return (AllTransactions)result;
            }
            catch (Exception ex)
            {
                CashBookLogger.LogError(ex, $"Inside TransactionRepository - GetTransactionDetails {id}");
                Console.Error.WriteLine(ex.Message);
                throw;
            }
            
        }
        public IEnumerable<AllTransactions> GetAllTransaction(string type)
        {
            SqlParameter[] parameters = new SqlParameter[1];
            parameters[0] = new SqlParameter("@type", type);
            try
            {                
                var result = _appDb.AllTransactions.FromSqlRaw<AllTransactions>("GetAll_Transaction @type", parameters);                                     
                return (IEnumerable<AllTransactions>)result;
            }
            catch (Exception ex)
            {
                CashBookLogger.LogError(ex, $"Inside TransactionRepository-GetAll_{type}Transaction");
                Console.Error.WriteLine(ex.Message);
                throw;
            }
        }       
    }
}
