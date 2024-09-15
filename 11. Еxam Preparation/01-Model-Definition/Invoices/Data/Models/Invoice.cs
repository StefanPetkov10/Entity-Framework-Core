using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Invoices.Data.Models.Enums;

namespace Invoices.Data.Models
{
    public class Invoice
    {
        [Key]
        public int Id { get; set; }

        public int Number { get; set; }

        public DateTime IssueDate { get; set; } //DATETIME2 -> By default is NULLABLE - required

        public DateTime DueDate { get; set; }

        public decimal Amount { get; set; }

        public CurrencyType CurrencyType { get; set; }

        [Required]
        [ForeignKey(nameof(Client))]
        public int ClientId { get; set; }
        [Required]
        public virtual Client Client { get; set; } = null!;
    }
}
