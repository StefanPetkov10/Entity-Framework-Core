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
            string inputJson = File.ReadAllText("../../../Datasets/users.json");

            string result = ImportUsers(context, inputJson);
            Console.WriteLine(result);


            //mapper = new Mapper(new MapperConfiguration(cfg =>
            //{
            //    cfg.AddProfile<ProductShopProfile>();
            //    //.CreateMapper());
            //}));
        }
        public static string ImportUsers(ProductShopContext context, string inputJson)
        {
            IMapper mapper = new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ProductShopProfile>();
            }));

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
    }
}