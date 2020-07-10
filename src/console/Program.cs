using System;
using System.Linq;
using System.Collections.Generic;
using Tests;
using Xunit.Runners;

// oneliner
// docker run -it --rm mcr.microsoft.com/dotnet/core/sdk:3.1-bionic /bin/bash -c "git clone https://github.com/cduplantis/users-example.git \
// && cd ./users-example && dotnet test ./src/console/Users.csproj && dotnet run -p ./src/console/Users.csproj

/// <summary>
/// A sample user program
/// </summary>
/// <remarks>
/// This was an excercise to improve a sample application with while maintaining a minimal stack/dependencies.
/// I am making some assumptions about the value of the application, its entitlement(replacements), costs, etc. 
/// Depending on a lifecycle assessment of the application, I could perhaps make different choices about the changes
/// that I have peformed
/// 
/// To run this, you'll need a .csproj as follows (xUnit likes to insert it's own static Main in the build.)
/// <![CDATA[ 
/// <Project Sdk="Microsoft.NET.Sdk">
///  <PropertyGroup>
///    <TargetFramework>netcoreapp3.1</TargetFramework>
///    <GenerateProgramFile>false</GenerateProgramFile>
///  </PropertyGroup>
///  <ItemGroup>
///    <PackageReference Include="xunit" Version="2.4.1" />
///    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="16.5.0" />
///    <PackageReference Include="xunit.runner.utility" Version="2.4.1" />
///    <PackageReference Include="xunit.runner.visualstudio" Version="2.4.1" />
///    <PackageReference Include="coverlet.collector" Version="1.2.0" />
///  </ItemGroup>
/// </Project>
/// ]]>
/// 
/// To run this, save this file to a directory, copy and paste the above section into a .csproj file and at the console in the
/// directory of this file run the following command to restore, test and run the application.
/// <code>
/// dotnet run -- test
/// </code>
/// 
/// The full source of this project can be found at https://github.com/cduplantis/users-example.git
///</remarks>
public class Program
{
    public static void Main(string[] args)
    {
        // if the "test" argument is passed, we run the unit tests and exit.
        if (args?.Any(s => string.Equals("test", s, StringComparison.InvariantCultureIgnoreCase)) ?? false)
        {
            SelfTester.Run();
            return;
        }

        var userService = new UserService(getAllUsers);
        var console = new Console();
        var printer = new Printer(console);

        console.WriteLine("Hello World");

        var developersAtAwesomeSauceInc = userService.FindUser(job: "developer", company: "awesome sauce inc.");
        printer.PrintOutResults("Developers at Awesome Sauce Inc.", developersAtAwesomeSauceInc);

        var peopleAtPandance = userService.FindUser(company: "pandance");
        printer.PrintOutResults("People at Pandance", peopleAtPandance);

        var developersWithLastNameOfSmithWhoWorkAtAwesomeSauceInc =
            userService.FindUser("Smith", "developer", "awesome sauce inc.");
        printer.PrintOutResults("Developers With Last Name of Smith Who Work at Awesome Sauce Inc.",
            developersWithLastNameOfSmithWhoWorkAtAwesomeSauceInc);
    }


    /// <summary>
    /// Get a list of users
    /// </summary>
    /// <returns>A list of users</returns>
    /// <remarks>
    /// This should ideally be using a repository pattern of or some abstraction of persistence.
    /// </remarks>
    private static List<User> getAllUsers()
    {
        return new List<User>
        {
            new User {Id = 1, Name = "Bob Smith", Job = "developer", Company = "awesome sauce inc."},
            new User {Id = 2, Name = "Barb Tillo", Job = "developer", Company = "awesome sauce inc."},
            new User {Id = 3, Name = "May Axix", Job = "product owner", Company = "pandance"},
            new User {Id = 4, Name = "Jane Heartily", Job = "developer", Company = "awesome sauce inc."},
            new User {Id = 5, Name = "Jim Kronn", Job = "developer", Company = "pandance"},
            new User {Id = 6, Name = "Kelly Cruther", Job = "developer", Company = "pandance"},
            new User {Id = 7, Name = "Mark Smith", Job = "product owner", Company = "awesome sauce inc."},
        };
    }
}


/// <summary>
/// Wrapper for the system console output
/// </summary>
public interface IConsole
{
    //
    // Summary:
    //     Writes the specified string value, followed by the current line terminator, to
    //     the standard output stream.
    //
    // Parameters:
    //   value:
    //     The value to write.
    //
    // Exceptions:
    //   T:System.IO.IOException:
    //     An I/O error occurred.
    void WriteLine(string output = null);
}

/// <inheritdoc cref="IConsole"/>
public class Console : IConsole
{
/// <inheritdoc cref="IConsole.WriteLine"/>
    public void WriteLine(string output = null) => System.Console.WriteLine(output);
}

/// <summary>
/// A results printer
/// </summary>
public class Printer
{
    private readonly IConsole Console;

    public Printer(IConsole console) => this.Console = console ?? new Console();


    /// <summary>
    /// Prints the user list to the console
    /// </summary>
    /// <param name="message">A message/banner to display</param>
    /// <param name="results">The list of users to display</param>
    public void PrintOutResults(string message, IList<User> results)
    {
        Console.WriteLine();
        Console.WriteLine("Results: " + message);
        foreach (var user in results)
        {
            Console.WriteLine(string.Format("ID: {0}, Name: {1}, Job: {2}, Company: {3}", user.Id, user.Name, user.Job,
                user.Company));
        }
    }
}

/// <summary>
/// Provides access to the known list of users.
/// </summary>
public class UserService
{
    Func<List<User>> userProvider;

    public UserService(Func<List<User>> userProvider)
    {
        this.userProvider = userProvider;
    }

    /// <summary>
    /// Filters the list of users returning those that match all of the providing non-null, non-empty criteria.
    /// </summary>
    /// <param name="name">the name of the employee to filter</param>
    /// <param name="job">the job description or title to filter</param>
    /// <param name="company">the company or title to filter</param>
    /// <returns>A list of matching users</returns>
    /// <remarks>
    /// This will match the <paramref name="job"/> and<paramref name="company"/> in entirety; whereas name is a partial (EndsWith). Null or empty string values
    /// are ignored from the filter.
    ///
    /// All string comparisons are performed using culture independent comparisons (<see cref="StringComparison.InvariantCulture"/>).
    /// </remarks>
    public IList<User> FindUser(string name = null, string job = null, string company = null)
    {
        if (string.IsNullOrWhiteSpace(company))
        {
            throw new ArgumentException("Company name is required.", "company");
        }

        return userProvider().FindAll(user =>
            (string.IsNullOrWhiteSpace(name) || user.Name.EndsWith(name, StringComparison.InvariantCulture))
            && (string.IsNullOrWhiteSpace(job) || user.Job.Equals(job, StringComparison.InvariantCulture))
            && user.Company.Equals(company, StringComparison.InvariantCulture));
    }
}


/// <summary>
/// Represents a user
/// </summary>
public class User
{
    /// <summary>
    /// Id of the user
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Name of the user
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Job or title that the user holds at a company
    /// </summary>
    public string Job { get; set; }

    /// <summary>
    /// The company at which the employee works
    /// </summary>
    public string Company { get; set; }
}


/// <summary>
/// Embedded unit tests for this utility. These can be run by passing "test" as argument to  <see cref="Program.Main"/> which will call the  <see cref="SelfTester"/>
/// </summary>
namespace Tests
{
    using System.Text;
    using System.Threading;
    using Xunit;

    public class UserService_Tests
    {
        private UserService userService;

        private readonly Func<List<User>> emptyUsers = () => new List<User>();

        private readonly Func<List<User>> defaultUsers = () => new List<User>()
        {
            new User {Id = 1, Name = "Mark", Job = "barista", Company = "*bx"},
            new User {Id = 2, Name = "Jane", Job = "manager", Company = "*bx"},
            new User {Id = 3, Name = "Alex", Job = "cooper", Company = "Barrels-r-Us"},
            new User {Id = 4, Name = "Jason Borne", Job = "agent", Company = "ABC"},
        };

        public UserService_Tests()
        {
            userService = new UserService(defaultUsers);
        }

        [Theory()]
        [InlineData("Mark", null, "")]
        [InlineData("Mark", null, null)]
        public void FindUser_throws_argumentexception_when_company_name_is_missing_or_null(string n, string j, string c)
        {
            var ex = Assert.Throws<ArgumentException>(() => userService.FindUser(name: n, job: j, company: c));
            Assert.Equal("Company name is required. (Parameter 'company')", ex.Message);
        }

        [Theory()]
        [InlineData("Mark", null, "*bx", 1)]
        [InlineData(null, "manager", "*bx", 1)]
        [InlineData(null, null, "*bx", 2)]
        [InlineData("Alex", null, "Barrels-r-Us", 1)]
        [InlineData("Alex", "cooper", "Barrels-r-Us", 1)]
        [InlineData("Alex", null, "*bx", 0)]
        [InlineData("Borne", null, "ABC", 1)]
        public void FindUser_performs_and_logic_of_parameters(string n, string j, string c, int count)
        {
            var result = userService.FindUser(name: n, job: j, company: c);
            Assert.NotNull(result);
            Assert.Equal(count, result.Count);
        }

        [Theory()]
        [InlineData("Alex", null, "*ABC", 0)]
        [InlineData("Borne", null, "ABC", 1)]
        public void FindUser_matches_end_of_string(string n, string j, string c, int count)
        {
            var result = userService.FindUser(name: n, job: j, company: c);
            Assert.NotNull(result);
            Assert.Equal(count, result.Count);
        }

    }

    public class Printer_Tests
    {
        private class DummyConsole : IConsole
        {
            StringBuilder stringBuilder = new StringBuilder();

            public void WriteLine(string output = null)
            {
                stringBuilder.AppendLine(output);
            }

            public override string ToString()
            {
                return stringBuilder.ToString();
            }
        }

        [Fact]
        public void Printer_PrintsCorrectly()
        {
            var users = new List<User>()
            {
                new User {Id = 1, Name = "Mark", Job = "barista", Company = "*bx"},
                new User {Id = 2, Name = "Jane", Job = "manager", Company = "*bx"},
                new User {Id = 3, Name = "Alex", Job = "cooper", Company = "Barrels-r-Us"},
            };

            // arrange
            var dc = new DummyConsole();
            var printer = new Printer(dc);

            printer.PrintOutResults("bam", users);

            Assert.Equal(@"
Results: bam
ID: 1, Name: Mark, Job: barista, Company: *bx
ID: 2, Name: Jane, Job: manager, Company: *bx
ID: 3, Name: Alex, Job: cooper, Company: Barrels-r-Us
", dc.ToString());
        }
    }


    /// <summary>
    /// An embededded unit test runner
    /// </summary>
    class SelfTester
    {
        static object consoleLock = new object();

        static ManualResetEvent finished = new ManualResetEvent(false);

        // Start out assuming success; we'll set this to 1 if we get a failed test
        static int result = 0;

        public static bool Run()
        {
            using (var runner =
                AssemblyRunner.WithoutAppDomain(System.Reflection.Assembly.GetExecutingAssembly().Location))
            {
                runner.OnDiscoveryComplete = (info) =>
                {
                    lock (consoleLock)
                        System.Console.WriteLine(
                            $"Running {info.TestCasesToRun} of {info.TestCasesDiscovered} tests...");
                };

                runner.OnExecutionComplete = (info) =>
                {
                    lock (consoleLock)
                        System.Console.WriteLine(
                            $"Finished: {info.TotalTests} tests in {Math.Round(info.ExecutionTime, 3)}s ({info.TestsFailed} failed, {info.TestsSkipped} skipped)");

                    finished.Set();
                };

                runner.OnTestFailed = (info) =>
                {
                    lock (consoleLock)
                    {
                        System.Console.ForegroundColor = ConsoleColor.Red;

                        System.Console.WriteLine("[FAIL] {0}: {1}", info.TestDisplayName, info.ExceptionMessage);
                        if (info.ExceptionStackTrace != null)
                            System.Console.WriteLine(info.ExceptionStackTrace);

                        System.Console.ResetColor();
                    }

                    result = 1;
                };

                runner.OnTestSkipped = (info) =>
                {
                    lock (consoleLock)
                    {
                        System.Console.ForegroundColor = ConsoleColor.Yellow;
                        System.Console.WriteLine("[SKIP] {0}: {1}", info.TestDisplayName, info.SkipReason);
                        System.Console.ResetColor();
                    }
                };

                System.Console.WriteLine("Running internal tests...");
                runner.Start();

                finished.WaitOne();
                finished.Dispose();
            }

            return result == 0;
        }
    }
}