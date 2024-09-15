namespace Invoices.DataProcessor
{
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Text;
    using Invoices.Data;
    using Invoices.Data.Models;
    using Invoices.Data.Models.Enums;
    using Invoices.DataProcessor.ImportDto;
    using Invoices.Utilities;
    using Newtonsoft.Json;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedClients
            = "Successfully imported client {0}.";

        private const string SuccessfullyImportedInvoices
            = "Successfully imported invoice with number {0}.";

        private const string SuccessfullyImportedProducts
            = "Successfully imported product - {0} with {1} clients.";


        public static string ImportClients(InvoicesContext context, string xmlString)
        {
            StringBuilder sb = new StringBuilder();

            XmlHelper xmlHelper = new XmlHelper();
            const string xmlRoot = "Clients";

            //Valid models to import into the DB
            ICollection<Client> clientsToImport = new List<Client>();

            ImportClientDto[] clientsDtos =
                xmlHelper.Deserialize<ImportClientDto[]>(xmlString, xmlRoot);

            foreach (var clientDto in clientsDtos)
            {
                if (!IsValid(clientDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                ICollection<Address> addressesToImpoert = new List<Address>();

                foreach (var addressDto in clientDto.Addresses)
                {
                    if (!IsValid(addressDto))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    Address address = new Address
                    {
                        StreetName = addressDto.StreetName,
                        StreetNumber = addressDto.StreetNumber,
                        PostCode = addressDto.PostCode,
                        City = addressDto.City,
                        Country = addressDto.Country
                    };

                    addressesToImpoert.Add(address);
                }

                Client client = new Client
                {
                    Name = clientDto.Name,
                    NumberVat = clientDto.NumberVat,
                    Addresses = addressesToImpoert
                };

                clientsToImport.Add(client);
                sb.AppendLine(string.Format(SuccessfullyImportedClients, clientDto.Name));
            }

            context.Clients.AddRange(clientsToImport);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }


        public static string ImportInvoices(InvoicesContext context, string jsonString)
        {
            StringBuilder sb = new StringBuilder();

            ImportInvoiceDto[] invoicesDtos =
                JsonConvert.DeserializeObject<ImportInvoiceDto[]>(jsonString);

            ICollection<Invoice> invoicesToImport = new List<Invoice>();
            //ICollection<int> validClients = context.Clients.Select(cl => cl.Id).ToList();
            foreach (var invoiceDto in invoicesDtos)
            {
                if (!IsValid(invoiceDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                bool isIssueDateValid = DateTime.TryParse(invoiceDto.IssueDate,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out DateTime issueDate);

                bool isDueDateValid = DateTime.TryParse(invoiceDto.DueDate,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out DateTime DueDate);

                if (!isIssueDateValid || !isDueDateValid ||
                    DateTime.Compare(DueDate, issueDate) < 0)
                {
                    //DateTime.Compare(t1, t2)
                    //Less than zero t1 is earlier than t2. -> -1 t1 before t2
                    //Zero t1 is the same as t2.
                    //Greater than zero t1 is later than t2. -> 1 t1 after t2
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                if (!context.Clients.Any(c => c.Id == invoiceDto.ClientId))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Invoice invoice = new Invoice
                {
                    Number = invoiceDto.Number,
                    IssueDate = issueDate,
                    DueDate = DueDate,
                    Amount = invoiceDto.Amount,
                    CurrencyType = (CurrencyType)invoiceDto.CurrencyType,
                    ClientId = invoiceDto.ClientId
                };

                invoicesToImport.Add(invoice);
                sb.AppendLine(string.Format(SuccessfullyImportedInvoices, invoiceDto.Number));
            }

            context.Invoices.AddRange(invoicesToImport);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportProducts(InvoicesContext context, string jsonString)
        {
            StringBuilder sb = new();
            ICollection<Product> productsToImport = new List<Product>();

            ImportProductDto[] deserializedProducts =
                JsonConvert.DeserializeObject<ImportProductDto[]>(jsonString)!;

            foreach (var productDto in deserializedProducts)
            {
                if (!IsValid(productDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Product newProduct = new Product()
                {
                    Name = productDto.Name,
                    Price = productDto.Price,
                    CategoryType = (CategoryType)productDto.CategoryType
                };

                ICollection<ProductClient> productClientsToImport = new List<ProductClient>();

                foreach (int clientId in productDto.Clients.Distinct())
                {
                    if (!context.Clients.Any(cl => cl.Id == clientId))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    ProductClient newProductClient = new ProductClient()
                    {
                        Product = newProduct,
                        ClientId = clientId
                    };

                    productClientsToImport.Add(newProductClient);
                }
                newProduct.ProductsClients = productClientsToImport;

                productsToImport.Add(newProduct);
                sb.AppendLine(string.Format(SuccessfullyImportedProducts, productDto.Name, productClientsToImport.Count));
            }
            context.Products.AddRange(productsToImport);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    }
}
