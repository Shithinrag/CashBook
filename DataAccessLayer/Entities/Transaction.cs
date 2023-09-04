using System.ComponentModel.DataAnnotations;

namespace DataAccessLayer.Entities

{
    public class Transaction
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        [StringLength(20)]
        public string? Description { get; set; }
        public int? CategoryId { get; set; }        
        public int? PaymentFromId { get; set; }       
        public int? PaymentToId { get; set; }        
        [StringLength(10)]
        public string Type { get; set; } = "";
        public DateTime CreatedDate { get; set; }
        public decimal Amount { get; set; }        
        public bool IsDelete { get; set; }

    }
    public class AllTransactions
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        [StringLength(20)]
        public string? Description { get; set; }
        public int? CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public int? PaymentFromId { get; set; }
        public string? PaymentFromName { get; set; }
        public int? PaymentToId { get; set; }
        public string? PaymentToName { get; set; }
        [StringLength(10)]
        public string Type { get; set; } = "";
        public DateTime CreatedDate { get; set; }
        public decimal Amount { get; set; }
        public bool IsDelete { get; set; }

    }   
}
