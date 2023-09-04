using DataAccessLayer.Abstraction;
using DataAccessLayer.Entities;
using Logger;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly AppDb _appDb;
        public PaymentRepository(AppDb appDb)
        {
            _appDb = appDb;

        }

        public void AddPayment(Payment payment)
        {
            SqlParameter[] parameters = new SqlParameter[5];

            parameters[0] = new SqlParameter("@name", payment.Name);
            parameters[1] = new SqlParameter("@opb", payment.OpeningBalance);
            parameters[2] = new SqlParameter("@isP", payment.IsPerson);
            parameters[3] = new SqlParameter("@isD", payment.IsDelete);
            try
            {
                _appDb.Database.ExecuteSqlRaw("Add_Payment @name,@opb,@isP,@isD", parameters);
            }
            catch (Exception ex)
            {
                CashBookLogger.LogError(ex, "Inside PaymentRepository- AddPayment");
                Console.Error.WriteLine(ex.Message);
            }
        }
        public void UpdatePayment(Payment payment)
        {
            SqlParameter[] parameters = new SqlParameter[5];
            parameters[0] = new SqlParameter("@id", payment.Id);
            parameters[1] = new SqlParameter("@name", payment.Name);
            parameters[2] = new SqlParameter("@opb", payment.OpeningBalance);
            parameters[3] = new SqlParameter("@isP", payment.IsPerson);
            parameters[4] = new SqlParameter("@isD", payment.IsDelete);
            try
            {
                _appDb.Database.ExecuteSqlRaw("Update_Payment @id,@name,@opb,@isP,@isD", parameters);
            }
            catch (Exception ex)
            {
                CashBookLogger.LogError(ex, "Inside PaymentRepository-UpdatePayment");
                Console.Error.WriteLine(ex.Message);
            }

        }
        public void ToggleStatus(int id)         
        {
            Payment payment = GetPaymentnDetails(id);
            payment.IsDelete = !payment.IsDelete;
            UpdatePayment(payment);
        }
        public Payment GetPaymentnDetails(int id)
        {

            SqlParameter[] parameters = new SqlParameter[1];
            parameters[0] = new SqlParameter("@id", id);           
            try
            {
                var result = _appDb.Payment.FromSqlRaw<Payment>("GetDetails_Payment @id", parameters).AsEnumerable().FirstOrDefault();
                result ??= new Payment();
                return (Payment)result;
            }
            catch (Exception ex)
            {
                CashBookLogger.LogError(ex, "Inside PaymentRepository-GetPaymentnDetails");
                Console.Error.WriteLine(ex.Message);
                throw;
            }

        }
        public IEnumerable<Payment> GetAllPayment()
        {

            try
            {
                var result = _appDb.Payment.FromSqlRaw<Payment>("GetAll_Payment");
                return (IEnumerable<Payment>)result;
            }
            catch (Exception ex)
            {
                CashBookLogger.LogError(ex, "Inside PaymentRepository- GetAllPayment");
                Console.Error.WriteLine(ex.Message);
                throw;
            }

        }
        public bool CheckDuplicatePayment(string name, int? id = null)
        {
            if (id == null)
            {
                if (_appDb.Payment.Any(p => p.Name == name))
                {
                    return false;
                }
                return true;
            }
            if (_appDb.Payment.Any(p => p.Id != id && p.Name == name))
            {
                return false;
            }
            return true;           

        }
        public IEnumerable<PaymentList> GetActivePayments()
        {
            try
            {
                var result = _appDb.Payment
                    .Where(c => c.IsDelete == false).Select(c => new PaymentList { Id = c.Id, Name = c.Name }).ToList();

                return (IEnumerable<PaymentList>)result;
            }
            catch (Exception ex)
            {
                CashBookLogger.LogError(ex, "Inside PaymentRepository- GetActivePayments");
                Console.Error.WriteLine(ex.Message);
                throw;
            }
        }
    }
}

