using System.ComponentModel.DataAnnotations;
using Invoices.Data.Models.Enums;

namespace Invoices.Data.Models
{
    public class Product
    {
        public Product()
        {
            this.ProductsClients = new HashSet<ProductClient>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(DataConstrains.ProductNameMaxLength)]
        public string Name { get; set; } = null!;

        //[Range(typeof(decimal), DataConstrains.ProductPriceMinValue, DataConstrains.ProductPriceMaxValue)]
        public decimal Price { get; set; }

        public CategoryType CategoryType { get; set; }

        public virtual ICollection<ProductClient> ProductsClients { get; set; }
    }
}
