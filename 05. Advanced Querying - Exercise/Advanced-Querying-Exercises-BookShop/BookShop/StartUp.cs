namespace BookShop
{
    public class StartUp
    {
        public static void Main()
        {
            using var dbContext = new BookShopContext();
            //DbInitializer.ResetDatabase(dbContext);

            //string input = Console.ReadLine();
            string result = GetBooksByPrice(dbContext);
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
        public static string GetBooksByPrice(BookShopContext context)
        {
            var books = context.Books
                .Where(b => b.Price > 40)
                .OrderByDescending(b => b.Price)
                .ToList();

            return string.Join(Environment.NewLine, books.Select(b => $"{b.Title} - ${b.Price:f2}"));
        }

        //Problem 5
        public static string GetBooksNotReleasedIn(BookShopContext context, int year)
        {
            var books = context.Books
                .Where(b => b.ReleaseDate.HasValue &&
                            b.ReleaseDate.Value.Year != year)
                .ToList();

            return string.Join(Environment.NewLine, books.Select(b => b.Title));
        }

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
        public static string GetBooksReleasedBefore(BookShopContext context, string date)
        {
            var parsedDate = DateTime.ParseExact(date, "dd-MM-yyyy", CultureInfo.InvariantCulture);

            var books = context.Books
                .Where(b => b.ReleaseDate < parsedDate)
                .Select(b => new
                {
                    b.Title,
                    b.EditionType,
                    b.Price,
                    b.ReleaseDate
                })
                .OrderByDescending(b => b.ReleaseDate);


            return string.Join(Environment.NewLine,
                books.Select(b => $"{b.Title} - {b.EditionType} - ${b.Price:f2}"));
        }

        //Problem 8
        public static string GetAuthorNamesEndingIn(BookShopContext context, string input)
        {
            string[] authorNames = context.Authors
                .Where(a => a.FirstName.EndsWith(input))
                .OrderBy(a => a.FirstName)
                .ThenBy(a => a.LastName)
                .Select(a => $"{a.FirstName} {a.LastName}")
                .ToArray();

            return string.Join(Environment.NewLine, authorNames);
        }

        //Problem 9
        public static string GetBookTitlesContaining(BookShopContext context, string input)
        {
            // Option 1
            //var books = context.Books
            //    .Where(b => b.Title.ToLower().Contains(input.ToLower()))
            //    .Select(b => new
            //    {
            //        b.Title
            //    })
            //    .OrderBy(b => b.Title);

            //return string.Join(Environment.NewLine, books.Select(b => b.Title));


            // Option 2
            var books = context.Books
                .Where(b => EF.Functions.Like(b.Title, $"%{input}%"))
                .Select(b => new
                {
                    b.Title
                })
                .OrderBy(b => b.Title);

            return string.Join(Environment.NewLine, books.Select(b => b.Title));
        }

        //Problem 10

        //Problem 11

        //Problem 12
        public static string CountCopiesByAuthor(BookShopContext context)
        {
            var authorsWithBookCopies = context.Authors
                .Select(a => new
                {
                    FullName = a.FirstName + " " + a.LastName,
                    TotalCopies = a.Books
                        .Sum(b => b.Copies)
                })
                //.Select(a => $"{a.FullName} - {a.TotalCopies}")
                .ToArray()
                .OrderByDescending(a => a.TotalCopies);

            //return string.Join(Environment.NewLine, authorsWithBookCopies);

            //Or
            StringBuilder sb = new();
            foreach (var item in authorsWithBookCopies)
            {
                sb.AppendLine($"{item.FullName} - {item.TotalCopies}");
            }

            return sb.ToString().TrimEnd();
        }

        //Problem 13
        public static string GetTotalProfitByCategory(BookShopContext context)
        {
            StringBuilder sb = new();

            var categoriesWithProfit = context.Categories
                .Select(c => new
                {
                    CategoryName = c.Name,
                    TotalProfit = c.CategoryBooks
                        .Sum(cb => cb.Book.Copies * cb.Book.Price)
                })
                .OrderByDescending(c => c.TotalProfit)// can be after ToArray()
                .ThenBy(c => c.CategoryName)//it might be faster
                .ToArray();

            foreach (var item in categoriesWithProfit)
            {
                sb.AppendLine($"{item.CategoryName} ${item.TotalProfit:F2}");
            }

            return sb.ToString().TrimEnd();
        }

        //Problem 14
        //public static string GetMostRecentBooks(BookShopContext context)
        //{

        //}

        //Problem 15

        //Problem 16
    }
}


