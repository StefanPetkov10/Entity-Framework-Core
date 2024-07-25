using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using ProductShop.Data;
using ProductShop.DTOs.Export;
using ProductShop.DTOs.Import;
using ProductShop.Models;

namespace ProductShop
{
    public class StartUp
    {
        //private static IMapper mapper;
        public static void Main()
        {
            ProductShopContext context = new ProductShopContext();

            //string inputJson = 
            //    File.ReadAllText("../../../Datasets/categories-products.json");

            string result = GetSoldProducts(context);
            Console.WriteLine(result);

            //mapper = new Mapper(new MapperConfiguration(cfg =>
            //{
            //    cfg.AddProfile<ProductShopProfile>();
            //    //.CreateMapper());
            //}));
        }


        //Problim 01
        //Method that add users with foreach loop
        //We have validation for the users
        public static string ImportUsers(ProductShopContext context, string inputJson)
        {
            IMapper mapper = MapperMethod();

            ImportUserDto[] userDtios =
                JsonConvert.DeserializeObject<ImportUserDto[]>(inputJson);

            ICollection<User> ValidUsers = new HashSet<User>();
            foreach (var userDto in userDtios)
            {
                User user = mapper.Map<User>(userDto);
                ValidUsers.Add(user);
            }

            context.Users.AddRange(ValidUsers);
            context.SaveChanges();

            return $"Successfully imported {ValidUsers.Count}";
        }

        //Problim 02
        //Method that add products without foreach loop
        public static string ImportProducts(ProductShopContext context, string inputJson)
        {
            IMapper mapper = MapperMethod();

            ImportProductDto[] productDtos =
                JsonConvert.DeserializeObject<ImportProductDto[]>(inputJson);

            Product[] products = mapper.Map<Product[]>(productDtos);

            context.Products.AddRange(products);
            context.SaveChanges();

            return $"Successfully imported {products.Length}";
        }

        //Problim 03
        public static string ImportCategories(ProductShopContext context, string inputJson)
        {
            IMapper mapper = MapperMethod();

            ImportCategoryDto[] categoryDtos =
                JsonConvert.DeserializeObject<ImportCategoryDto[]>(inputJson);

            ICollection<Category> validCategories = new HashSet<Category>();

            foreach (var categoryDto in categoryDtos)
            {
                if (String.IsNullOrEmpty(categoryDto.Name))
                {
                    continue;
                }

                Category category = mapper.Map<Category>(categoryDto);
                validCategories.Add(category);
            }

            context.Categories.AddRange(validCategories);
            context.SaveChanges();

            return $"Successfully imported {validCategories.Count}";
        }

        //Problim 04
        public static string ImportCategoryProducts(ProductShopContext context, string inputJson)
        {
            IMapper mapper = MapperMethod();

            ImportCategoryProductDto[] categoryProductDtos =
                  JsonConvert.DeserializeObject<ImportCategoryProductDto[]>(inputJson);

            ICollection<CategoryProduct> validEnries = new HashSet<CategoryProduct>();

            foreach (var cpDto in categoryProductDtos)
            {
                if (!context.Categories.Any(c => c.Id == cpDto.CategoryId) ||
                    !context.Products.Any(p => p.Id == cpDto.ProductId))
                {
                    continue;
                }

                CategoryProduct categoryProduct =
                    mapper.Map<CategoryProduct>(cpDto);

                validEnries.Add(categoryProduct);
            }

            context.CategoriesProducts.AddRange(validEnries);
            context.SaveChanges();

            return $"Successfully imported {validEnries.Count}";
        }

        //Problim 05.1
        //Anonymous object + Manual Mapping
        //DTO + Manual Mapping
        //DTO + AutoMapper
        public static string GetProductsInRange(ProductShopContext context)
        {
            //#Anonymous object + Manual Mapping
            var products = context.Products
                .Where(p => p.Price >= 500 && p.Price <= 1000)
                .OrderBy(p => p.Price)
                .Select(p => new
                {
                    name = p.Name,
                    price = p.Price,
                    seller = p.Seller.FirstName + " " + p.Seller.LastName
                })
                .AsNoTracking()
                .ToArray();

            return JsonConvert.SerializeObject(products, Formatting.Indented);
        }

        //Problim 05.2
        public static string GetProductsInRange1(ProductShopContext context)
        {
            //#DTO + AutoMapper
            IMapper mapper = MapperMethod();

            ExportProductRangeDto[] productDtos = context.Products
                .Where(p => p.Price >= 500 && p.Price <= 1000)
                .OrderBy(p => p.Price)
                .AsNoTracking()
                .ProjectTo<ExportProductRangeDto>(mapper.ConfigurationProvider)
                .ToArray();

            return JsonConvert.SerializeObject(productDtos, Formatting.Indented);
        }

        //Problim 06
        public static string GetSoldProducts(ProductShopContext context)
        {
            IContractResolver contractResolver = ConfigureCamelCaseNaming();

            var usersWithSoldProducts = context.Users
                .Where(u => u.ProductsSold.Any(ps => ps.Buyer != null))
                .OrderBy(u => u.LastName)
                .ThenBy(u => u.FirstName)
                .Select(u => new
                {
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    SoldProducts = u.ProductsSold
                        .Where(ps => ps.Buyer != null)
                        .Select(ps => new
                        {
                            Name = ps.Name,
                            Price = ps.Price,
                            BuyerFirstName = ps.Buyer.FirstName,
                            BuyerLastName = ps.Buyer.LastName
                        })
                        .ToArray()
                })
                .AsNoTracking()
                .ToArray();

            return JsonConvert.SerializeObject(usersWithSoldProducts,
                Formatting.Indented,
                new JsonSerializerSettings()
                {
                    ContractResolver = contractResolver
                });
        }


        private static IMapper MapperMethod()
        {
            return new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ProductShopProfile>();
            }));
        }
        private static IContractResolver ConfigureCamelCaseNaming()
        {
            return new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy(false, true)
            };
        }
    }
}