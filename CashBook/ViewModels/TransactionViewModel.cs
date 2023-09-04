using System.ComponentModel.DataAnnotations;

namespace CashBook.ViewModels
{
    public class TransactionViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Date is Required")]
        public string Date { get; set; }

        [StringLength(20, ErrorMessage = "Description length should not be greater than 20")]
        [Required(ErrorMessage = "Description is Required")]        
        public string? Description { get; set; }

        public int? CategoryIdExpense { get; set; } //NonContra
        public string? CategoryNameExpense { get; set; } //NonContra
        public int? CategoryIdIncome { get; set; } //NonContra
        public string? CategoryNameIncome { get; set; } //NonContra
        public int? PaymentFromIdSingle { get; set; } //NonContra
        public string? PaymentFromNameSingle { get; set; }//NonContra
        public int? PaymentFromIdContra { get; set; } //Contra
        public int? PaymentToId { get; set; } //Contra
        public string? PaymentToName { get; set; } //Contra

        [StringLength(10)]
        public string Type { get; set; } = "";
        public DateTime CreatedDate { get; set; }

        [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "Amount must be numeric and use up to two decimal places")]
        [Required(ErrorMessage = "Amount is Required")]        
        public decimal Amount { get; set; }
        public bool IsDelete { get; set; }
    }
    public class FirstNameValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null) // Checking for Empty Value
            {
                return new ValidationResult("Please Provide First Name");
            }
            else
            {
                if (value.ToString().Contains("@"))
                {
                    return new ValidationResult("First Name should Not contain @");
                }
            }
            return ValidationResult.Success;
        }
    }
}
