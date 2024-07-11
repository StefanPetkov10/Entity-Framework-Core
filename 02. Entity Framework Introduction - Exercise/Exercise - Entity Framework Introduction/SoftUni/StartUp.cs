using System.Globalization;
using System.Text;
using SoftUni.Data;
using SoftUni.Models;

public class StartUp
{
    public static void Main(string[] args)
    {
        SoftUniContext dbCntext = new SoftUniContext();

        string result = IncreaseSalaries(dbCntext);

        Console.WriteLine(result);
    }

    //Problem 03
    public static string GetEmployeesFullInformation(SoftUniContext context)
    {
        StringBuilder stringBuilder = new StringBuilder();

        var employees = context.Employees
            .OrderBy(e => e.EmployeeId)
            .Select(e => new
            {
                e.FirstName,
                e.LastName,
                e.MiddleName,
                e.JobTitle,
                Salary = e.Salary.ToString("f2")
            })
            //.AsNoTracking()
            .ToArray();

        foreach (var employee in employees)
        {
            stringBuilder.AppendLine($"{employee.FirstName} {employee.LastName} {employee.MiddleName} {employee.JobTitle} {employee.Salary}");
        }

        return stringBuilder.ToString().TrimEnd();
    }

    //Problem 04
    public static string GetEmployeesWithSalaryOver50000(SoftUniContext context)
    {
        var employees = context.Employees
            .Where(e => e.Salary > 50_000)
            .OrderBy(e => e.FirstName)
            .Select(e => new
            {
                e.FirstName,
                e.Salary
            });

        StringBuilder sb = new StringBuilder();

        foreach (var e in employees)
        {
            sb.AppendLine($"{e.FirstName} - {e.Salary:f2}");
        }

        return sb.ToString().TrimEnd();
    }

    //Problem 05
    public static string GetEmployeesFromResearchAndDevelopment(SoftUniContext context)
    {
        StringBuilder stringBuilder = new StringBuilder();

        var employeesRnD = context.Employees
            .Where(e => e.Department.Name == "Research and Development")
            .Select(e => new
            {
                e.FirstName,
                e.LastName,
                DepartmentName = e.Department.Name,
                e.Salary
            })
            .OrderBy(e => e.Salary)
            .ThenByDescending(e => e.FirstName)
            .ToArray();

        foreach (var e in employeesRnD)
        {
            stringBuilder
                .AppendLine($"{e.FirstName} {e.LastName} from {e.DepartmentName} - ${e.Salary:f2}");
        }

        return stringBuilder.ToString().TrimEnd();
    }

    //Problem 06
    public static string AddNewAddressToEmployee(SoftUniContext context)
    {
        Address newAddress = new Address()
        {
            AddressText = "Vitoshka 15",
            TownId = 4

        };
        //context.Addresses.Add(newAddress); // Add the new address to the context. This the way for adding into the database
        Employee? emploee = context.Employees
            .FirstOrDefault(e => e.LastName == "Nakov");

        emploee!.Address = newAddress;
        // ! - I am sure that the employee is not null

        context.SaveChanges();

        var employeesAddresses = context.Employees
            .OrderByDescending(e => e.AddressId)
            .Select(e => e.Address!.AddressText)
            .Take(10)
            .ToArray();

        return string.Join(Environment.NewLine, employeesAddresses);
    }

    //Problem 07
    public static string GetEmployeesInPeriod(SoftUniContext context)
    {
        StringBuilder stringBuilder = new StringBuilder();

        var employeesWithProject = context.Employees
            //.Where(e => e.EmployeesProjects
            //    .Any(ep => ep.Project.StartDate.Year >= 2001
            //            && ep.Project.StartDate.Year <= 2003))
            .Take(10)
            .Select(e => new
            {
                e.FirstName,
                e.LastName,
                ManagerFirstName = e.Manager!.FirstName,
                ManagerLastName = e.Manager!.LastName,
                Projects = e.EmployeesProjects
                    .Where(ep => ep.Project.StartDate.Year >= 2001
                        && ep.Project.StartDate.Year <= 2003)
                    .Select(ep => new
                    {
                        ProjectName = ep.Project.Name,
                        StartDate = ep.Project.StartDate.ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture),
                        EndDate = ep.Project.EndDate.HasValue ?
                                ep.Project.EndDate.Value.ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture) :
                                                                  "not finished"
                    })
                    .ToArray()
            })
            .ToArray();

        foreach (var e in employeesWithProject)
        {
            stringBuilder.AppendLine($"{e.FirstName} {e.LastName} - Manager: {e.ManagerFirstName} {e.ManagerLastName}");

            foreach (var project in e.Projects)
            {
                stringBuilder.AppendLine($"--{project.ProjectName} - {project.StartDate} - {project.EndDate}");
            }
        }

        return stringBuilder.ToString().TrimEnd();
    }

    //Problem 08
    public static string GetAddressesByTown(SoftUniContext context)
    {
        var addresses = context.Addresses
            .OrderByDescending(a => a.Employees.Count)
            .ThenBy(a => a.Town.Name)
            .ThenBy(a => a.AddressText)
            .Take(10)
            .Select(a => new
            {
                a.AddressText,
                TownName = a.Town.Name,
                EmployeeCount = a.Employees.Count
            })
            .ToArray();

        StringBuilder sb = new StringBuilder();

        foreach (var a in addresses)
        {
            sb.AppendLine($"{a.AddressText}, {a.TownName} - {a.EmployeeCount} employees");
        }

        return sb.ToString().TrimEnd();
    }

    //Problem 09
    public static string GetEmployee147(SoftUniContext context)
    {
        var employee147 = context.Employees
            .Where(e => e.EmployeeId == 147)
            .Select(e => new
            {
                e.FirstName,
                e.LastName,
                e.JobTitle,
                Projects = context.EmployeesProjects
                .OrderBy(ep => ep.Project.Name)
                .Select(ep => new
                {
                    ProjectName = ep.Project.Name
                })
                .ToArray()
            })
            .ToArray();

        StringBuilder sb = new StringBuilder();

        foreach (var e in employee147)
        {
            sb.AppendLine($"{e.FirstName} {e.LastName} - {e.JobTitle}");

            foreach (var p in e.Projects)
            {
                sb.AppendLine($"{p.ProjectName}");
            }
        }

        return sb.ToString().TrimEnd();
    }

    //Problem 10
    public static string GetDepartmentsWithMoreThan5Employees(SoftUniContext context)
    {
        StringBuilder sb = new StringBuilder();

        var departments = context.Departments
            .Where(d => d.Employees.Count > 5)
            .OrderBy(d => d.Employees.Count)
            .ThenBy(d => d.Name)
            .Select(d => new
            {
                d.Name,
                ManagerFirstName = d.Manager!.FirstName,
                ManagerLastName = d.Manager!.LastName,
                Employees = d.Employees
                .OrderBy(e => e.FirstName)
                .ThenBy(e => e.LastName)
                .Select(e => new
                {
                    EmployeeFirstName = e.FirstName,
                    EmployeeLastName = e.LastName,
                    e.JobTitle

                })
                .ToArray()
            })
            .ToArray();

        foreach (var depatment in departments)
        {
            sb
            .AppendLine($"{depatment.Name} - {depatment.ManagerFirstName} {depatment.ManagerLastName}");

            foreach (var employee in depatment.Employees)
            {
                sb
               .AppendLine($"{employee.EmployeeFirstName} {employee.EmployeeLastName} - {employee.JobTitle}");
            }
        }

        return sb.ToString().TrimEnd();
    }

    //Problem 11
    public static string GetLatestProjects(SoftUniContext context)
    {
        var projects = context.Projects
            .OrderByDescending(p => p.StartDate)
            .Take(10)
            .OrderBy(p => p.Name)
            .Select(p => new
            {
                p.Name,
                p.Description,
                StartDate = p.StartDate.ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture)
            });

        StringBuilder sb = new();

        foreach (var project in projects)
        {
            sb.AppendLine(project.Name);
            sb.AppendLine(project.Description);
            sb.AppendLine(project.StartDate.ToString(CultureInfo.InvariantCulture));
        }
        return sb.ToString().TrimEnd();
    }

    //Problem 12
    public static string IncreaseSalaries(SoftUniContext context)
    {
        string[] departmentsToIncrease = { "Engineering", "Tool Design", "Marketing", "Information Services" };

        //var departmentsToIncrease = context.Departments
        //.Where(d => d.Name == "Engineering" || d.Name == "Tool Design" || d.Name == "Marketing" || d.Name == "Information Services")
        //.ToString();

        var employessWithIncrease = context.Employees
            .Where(e => departmentsToIncrease.Contains(e.Department.Name))
            .OrderBy(e => e.FirstName)
            .ThenBy(e => e.LastName)
            .Select(e => new
            {
                e.FirstName,
                e.LastName,
                e.Salary
                //Salary = e.Salary + e.Salary * 0.12m
            })
            .ToArray();

        StringBuilder sb = new();

        foreach (var employee in employessWithIncrease)
        {
            decimal salary = employee.Salary;
            decimal newSalary = salary + salary * 0.12m;


            sb.AppendLine($"{employee.FirstName} {employee.LastName} (${newSalary:F2})");
        }

        return sb.ToString().TrimEnd();
    }

    //Problem 13
    public static string GetEmployeesByFirstNameStartingWithSa(SoftUniContext context)
    {
        var employees = context.Employees
            .Where(e => e.FirstName.StartsWith("Sa"))
            .OrderBy(e => e.FirstName)
            .ThenBy(e => e.LastName)
            .Select(e => new
            {
                e.FirstName,
                e.LastName,
                e.JobTitle,
                e.Salary
            })
            .ToArray();


        var sb = new StringBuilder();

        foreach (var e in employees)
        {
            sb.AppendLine($"{e.FirstName} {e.LastName} - {e.JobTitle} - (${e.Salary:F2})");
        }

        return sb.ToString().TrimEnd();
    }
    //Problem 14
    public static string DeleteProjectById(SoftUniContext context)
    {
        //Delete all rows from the EmployeesProjects that refer to Project with Id = 2
        IQueryable<EmployeeProject> epToDelete = context.EmployeesProjects
            .Where(ep => ep.ProjectId == 2);
        context.EmployeesProjects.RemoveRange(epToDelete);

        Project projectToDelete = context.Projects
            .Find(2)!;
        context.Projects.Remove(projectToDelete);

        string[] projects = context.Projects
            .Take(10)
            .Select(p => p.Name)
            .ToArray();

        return string.Join(Environment.NewLine, projects);
    }

    //Problem 15
    public static string RemoveTown(SoftUniContext context)
    {
        var town = context.Towns
            .FirstOrDefault(t => t.Name == "Seattle");

        var addressesInTown = context.Addresses
            .Where(a => a.TownId == town.TownId)
            .ToList();

        foreach (var address in addressesInTown)
        {
            var employeesAtAddress = context.Employees
                .Where(e => e.AddressId == address.AddressId)
                .ToList();

            foreach (var employee in employeesAtAddress)
            {
                employee.AddressId = null;
            }
        }

        context.Addresses.RemoveRange(addressesInTown);
        context.Towns.Remove(town);

        context.SaveChanges();

        return $"{addressesInTown.Count} addresses in Seattle were deleted";

    }
}




