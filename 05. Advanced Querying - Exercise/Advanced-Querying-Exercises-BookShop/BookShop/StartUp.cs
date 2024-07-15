namespace BookShop
{
    using System.Linq;
    using System.Text;
    using BookShop.Models.Enums;
    using Data;

    public class StartUp
    {
        public static void Main()
        {
            using var dbContext = new BookShopContext();
            //DbInitializer.ResetDatabase(dbContext);

            string input = Console.ReadLine();
            string result = GetBooksByCategory(dbContext, input);
            Console.WriteLine(result);

        }

        //Problem 2
        public static string GetBooksByAgeRestriction(BookShopContext dbContext, string command)
        {
            //bool hasParsed = Enum.TryParse(typeof(AgeRestriction), command, true, out object? ageRestrictionObject);
            //AgeRestriction ageRestriction;
            //if (hasParsed)
            //{
            //    ageRestriction = (AgeRestriction)ageRestrictionObject;

            //    string[] booksTitles = dbContext.Books
            //    //.Where(b => b.AgeRestriction.ToString().ToLower() == command.ToLower())
            //    .Where(b => b.AgeRestriction == ageRestriction)
            //    .OrderBy(b => b.Title)
            //    .Select(b => b.Title)
            //    .ToArray();

            //    return string.Join(Environment.NewLine, booksTitles);
            //}

            //return "Invalid command!";

            //Method 2
            try
            {
                AgeRestriction ageRestriction1 = Enum.Parse<AgeRestriction>(command, true);

                string[] booksTitles = dbContext.Books
                .Where(b => b.AgeRestriction == ageRestriction1)
                .OrderBy(b => b.Title)
                .Select(b => b.Title)
                .ToArray();

                return string.Join(Environment.NewLine, booksTitles);
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        //Problem 3
        public static string GetGoldenBooks(BookShopContext context)
        {
            string[] bookTitles = context.Books
                .Where(b => b.EditionType == EditionType.Gold && b.Copies < 5000)
                .OrderBy(b => b.BookId)
                .Select(b => b.Title)
                .ToArray();

            return string.Join(Environment.NewLine, bookTitles);
        }

        //Problem 4

        //Problem 5

        //Problem 6
        public static string GetBooksByCategory(BookShopContext context, string input)
        {
            string[] categories = input
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(c => c.ToLower())
                .ToArray();

            //string[] bookTitles = context.Books
            //    .Where(b => b.BookCategories
            //                .Any(bc => categories.Contains(bc.Category.Name.ToLower())))
            //    .Select(b => b.Title)
            //    .OrderBy(b => b)
            //    .ToArray();

            //return string.Join(Environment.NewLine, bookTitles);

            var bookTitles = context.BooksCategories
                .Where(c => categories.Contains(c.Category.Name.ToLower()))
                .Select(t => t.Book.Title)
                .OrderBy(b => b)
                .ToArray();

            StringBuilder sb = new();

            foreach (var title in bookTitles)
            {
                sb.AppendLine(title);
            }

            return sb.ToString().TrimEnd();
        }

        //Problem 7

        //Problem 8

        //Problem 9

        //Problem 10

        //Problem 11

        //Problem 12

        //Problem 13

        //Problem 14

        //Problem 15

        //Problem 16
    }
}


