using AutoMapper;
using Newtonsoft.Json;
using ProductShop.Data;
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
            string inputJson = File.ReadAllText("../../../Datasets/categories.json");

            string result = ImportCategories(context, inputJson);
            Console.WriteLine(result);

            //mapper = new Mapper(new MapperConfiguration(cfg =>
            //{
            //    cfg.AddProfile<ProductShopProfile>();
            //    //.CreateMapper());
            //}));
        }
        private static IMapper MapperMethod()
        {
            return new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ProductShopProfile>();
            }));
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
    }
}