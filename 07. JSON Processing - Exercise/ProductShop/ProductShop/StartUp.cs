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

            string result = GetUsersWithProducts(context);
            Console.WriteLine(result);

            //mapper = new Mapper(new MapperConfiguration(cfg =>
            //{
            //    cfg.AddProfile<ProductShopProfile>();
            //    //.CreateMapper());
            //}));
        }


        //Problem 01
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

        //Problem 02
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

        //Problem 03
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

        //Problem 04
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

        //Problem 05.1
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

        //Problem 05.2
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

        //Problem 06
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

        //Problem 07
        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            IContractResolver contractResolver = ConfigureCamelCaseNaming();

            var categories = context.Categories
                .OrderByDescending(c => c.CategoriesProducts.Count)
                .Select(c => new
                {
                    Category = c.Name,
                    ProductsCount = c.CategoriesProducts.Count,
                    AveragePrice = Math.Round((double)(c.CategoriesProducts.Any() ?
                    c.CategoriesProducts.Average(cp => cp.Product.Price) : 0), 2),
                    TotalRevenue = Math.Round((double)(c.CategoriesProducts.Any() ?
                    c.CategoriesProducts.Sum(cp => cp.Product.Price) : 0), 2)
                })
                .ToArray();

            return JsonConvert.SerializeObject(categories,
                Formatting.Indented,
                new JsonSerializerSettings()
                {
                    ContractResolver = contractResolver
                });
        }

        //Problem 08
        public static string GetUsersWithProducts(ProductShopContext context)
        {
            IContractResolver contractResolver = ConfigureCamelCaseNaming();

            var users = context.Users
                .Where(u => u.ProductsSold.Any(p => p.Buyer != null))
                .Select(u => new
                { // UserDto
                    u.FirstName,
                    u.LastName,
                    u.Age,
                    SoldProducts = new
                    { // ProductWrapperDto
                        Count = u.ProductsSold.Count(ps => ps.Buyer != null),
                        Products = u.ProductsSold
                            .Where(p => p.Buyer != null)
                            .Select(p => new
                            { // ProductDto
                                p.Name,
                                p.Price
                            })
                            .ToArray()
                    }
                })
                .OrderByDescending(u => u.SoldProducts.Count)
                .AsNoTracking()
                .ToArray();

            var userWrapperDto = new
            {
                UsersCount = users.Length,
                Users = users
            };

            return JsonConvert.SerializeObject(userWrapperDto,
                Formatting.Indented,
                new JsonSerializerSettings()
                {
                    ContractResolver = contractResolver,
                    NullValueHandling = NullValueHandling.Ignore
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