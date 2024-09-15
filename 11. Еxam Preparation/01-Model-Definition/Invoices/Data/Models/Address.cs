using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Invoices.Data.Models
{
    public class Address
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(DataConstrains.AddressStreetNameMaxLength)]
        public string StreetName { get; set; } = null!; //NVARCHAR(20)

        public int StreetNumber { get; set; }

        [Required]
        public string PostCode { get; set; } = null!; //NVARCHAR(MAX)

        [Required]
        [MaxLength(DataConstrains.AddressCityMaxLength)]
        public string City { get; set; } = null!; //NVARCHAR(15)

        [Required]
        [MaxLength(DataConstrains.AddressCountryMaxLength)]
        public string Country { get; set; } = null!; //NVARCHAR(15)

        [Required]
        [ForeignKey(nameof(Client))]
        public int ClientId { get; set; }

        [Required]
        public virtual Client Client { get; set; } = null!;
    }
}
