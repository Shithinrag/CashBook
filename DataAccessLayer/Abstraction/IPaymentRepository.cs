using DataAccessLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Abstraction
{
    public interface IPaymentRepository
    {
        public void AddPayment(Payment payment);
        public void UpdatePayment(Payment payment);
        public void ToggleStatus(int id);
        public IEnumerable<Payment> GetAllPayment();
        public Payment GetPaymentnDetails(int id);
        public bool CheckDuplicatePayment(string name, int? id = null);
        public IEnumerable<PaymentList> GetActivePayments();
    }
}
