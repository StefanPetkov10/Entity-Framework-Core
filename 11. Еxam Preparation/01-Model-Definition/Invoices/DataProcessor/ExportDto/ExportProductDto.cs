using Newtonsoft.Json;

namespace Invoices.DataProcessor.ExportDto
{
    public class ExportProductDto
    {
        [JsonProperty(nameof(Name))]
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public string Category { get; set; } = null!;
        public ExportProductClientsDto[] Clients { get; set; } = null!;
    }
}
