using System.ComponentModel.DataAnnotations;
using static Invoices.Data.DataConstrains;
namespace Invoices.DataProcessor.ImportDto
{
    public class ImportProductDto
    {
        [Required]
        [MinLength(ProductNameMinLength)]
        [MaxLength(ProductNameMaxLength)]
        public string Name { get; set; } = null!;

        [Range(typeof(decimal), ProductPriceMinValue, ProductPriceMaxValue)]
        public decimal Price { get; set; }

        [Range(ProductCategoryTypeMinValue, ProductCategoryTypeMaxValue)]
        public int CategoryType { get; set; }

        public int[] Clients { get; set; } = null!;
    }
}
