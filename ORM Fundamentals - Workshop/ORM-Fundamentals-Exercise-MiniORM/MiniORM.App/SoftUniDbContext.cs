using MiniORM.App.Entities;

namespace MiniORM.App
{
    public class SoftUniDbContext : DbContext
    {
        public SoftUniDbContext(string connctionString) : base(connctionString)
        {

        }

        public DbSet<Employee> Employees { get; }
        public DbSet<Department> Departments { get; }
    }
}
