using System.ComponentModel.DataAnnotations;
using static Invoices.Data.DataConstrains;

namespace Invoices.DataProcessor.ImportDto
{
    public class ImportInvoiceDto
    {
        [Range(InvoiceNumberMinValue, InvoiceNumberMaxValue)]
        public int Number { get; set; }

        [Required]
        public string IssueDate { get; set; } = null!; // DateTime -> Deserialize as a string

        [Required]
        public string DueDate { get; set; } = null!; // DateTime -> Deserialize as a string

        public decimal Amount { get; set; }

        [Range(InvoiceCurrencyTypeMinValue, InvoiceCurrencyTypeMaxValue)]
        public int CurrencyType { get; set; } // Enum -> Deserialize as an integer

        [Required]
        public int ClientId { get; set; }
    }
}
