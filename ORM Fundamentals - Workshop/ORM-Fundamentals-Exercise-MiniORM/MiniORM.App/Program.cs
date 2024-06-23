using MiniORM;
using MiniORM.App;
using MiniORM.App.Entities;
var departments = new Department[]
{
    new Department { Id = 1, Name = "Engineering" },
    new Department { Id = 2, Name = "Sales" }
};

const string connectionString = @"Server=.;Authentication=Windows Authentication;Database=SoftUni;TrustServerCertificate=True;";

var dbContext = new SoftUniDbContext(connectionString);

var changeTracker = new ChangeTracker<Department>(departments);

foreach (var (original, copy) in departments.Zip(changeTracker.AllEntities))
{
    Console.WriteLine(ReferenceEquals(original, copy));
}