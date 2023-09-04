using System.ComponentModel.DataAnnotations;

namespace DataAccessLayer.Entities

{
    public class Payment
    {
      
        public int Id { get; set; }           
        [StringLength(20)]
        public string Name { get; set; }= "";            
        public decimal OpeningBalance { get; set; }
        public bool IsPerson { get; set; }
        public bool IsDelete { get; set; }
    }
    public class PaymentList
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
    }
}

