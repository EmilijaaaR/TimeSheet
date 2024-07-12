using Domain.Entities;
using Domain.Enums;
using Infrastructure.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace API.Tests
{
    public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
    {
        private string _databaseName;

        public void SetDatabaseName(string databaseName)
        {
            _databaseName = databaseName;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Development");

            builder.ConfigureServices(services =>
            {
                var dbContextDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<TimeSheetDbContext>));
                if (dbContextDescriptor != null)
                {
                    services.Remove(dbContextDescriptor);
                }

                services.AddDbContext<TimeSheetDbContext>(options =>
                {
                    options.UseInMemoryDatabase(_databaseName ?? "TestDatabase");
                });

                services.AddAuthentication("TestScheme")
                        .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("TestScheme", options => { });

                services.AddSingleton<IAuthorizationHandler, AllowAnonymousAuthorizationHandler>();

                var sp = services.BuildServiceProvider();

                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<TimeSheetDbContext>();

                    db.Database.EnsureCreated();

                    SeedTestData(db);
                }
            });
        }

        public void ResetDatabase()
        {
            var options = new DbContextOptionsBuilder<TimeSheetDbContext>()
                .UseInMemoryDatabase(_databaseName ?? "TestDatabase")
                .Options;

            using (var context = new TimeSheetDbContext(options))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
                SeedTestData(context);
            }
        }
        private void SeedTestData(TimeSheetDbContext context)
        {
            var country1 = new Country { Id = 1, Name = "Country 1" };
            var country2 = new Country { Id = 2, Name = "Country 2" };
            var country3 = new Country { Id = 3, Name = "Country 3" };
            context.Countries.AddRange(country1, country2, country3);

            var client1 = new Client { Id = 1, Name = "Client 1", Address = "Address 1", City = "City 1", PostalCode = "11111", Country = country1 };
            var client2 = new Client { Id = 2, Name = "Client 2", Address = "Address 2", City = "City 2", PostalCode = "22222", Country = country2 };
            var client3 = new Client { Id = 3, Name = "Client 3", Address = "Address 3", City = "City 3", PostalCode = "33333", Country = country3 };
            var client4 = new Client { Id = 4, Name = "Client 4", Address = "Address 4", City = "City 4", PostalCode = "44444", Country = country3 };
            context.Clients.AddRange(client1, client2, client3, client4);

            var category1 = new Category { Id = 1, Name = "Test Category 1" };
            var category2 = new Category { Id = 2, Name = "Test Category 2" };
            var category3 = new Category { Id = 3, Name = "Test Category 3" };
            context.Categories.AddRange(category1, category2, category3);

            var project1 = new Project { Id = 1, Name = "Project 1", Description = "Description 1", Client = client1, Status = ProjectStatus.Active };
            var project2 = new Project { Id = 2, Name = "Project 2", Description = "Description 2", Client = client2, Status = ProjectStatus.Inactive };
            context.Projects.AddRange(project1, project2);

            var role1 = new Role { Id = 1, Name = "Admin" };
            var role2 = new Role { Id = 2, Name = "User" };
            context.Roles.AddRange(role1, role2);

            var user1 = new User("Test User1", "Test User1", "UsernameTest1", "hash1", "salt1");
            user1.Id = 1;
            var user2 = new User("Test User2", "Test User2", "UsernameTest2", "hash2", "salt2");
            user2.Id = 2;
            var user3 = new User("Test User3", "Test User3", "UsernameTest3", "hash3", "salt3");
            user3.Id = 3;
            context.Users.AddRange(user1, user2, user3);

            var timesheet1 = new Timesheet { Id = 1, UserId = 1, Project = project1, Category = category1, Date = new DateTime(2024, 6, 25), HoursWorked = 8, Description = "Timesheet 1", OverTime = 1 };
            var timesheet3 = new Timesheet { Id = 3, UserId = 1, Project = project1, Category = category1, Date = new DateTime(2024, 6, 26), HoursWorked = 9, Description = "Timesheet 3", OverTime = 3 };
            var timesheet2 = new Timesheet { Id = 2, UserId = 2, Project = project2, Category = category2, Date  = new DateTime(2024, 6, 30), HoursWorked = 7.5m, Description = "Timesheet 2", OverTime = 0.5m };
            var timesheet4 = new Timesheet { Id = 4, UserId = 2, Project = project2, Category = category2, Date  = new DateTime(2024, 6, 29), HoursWorked = 8.5m, Description = "Timesheet 4", OverTime = 2 };
            context.Timesheets.AddRange(timesheet1, timesheet2, timesheet3, timesheet4);

            var clientUser1 = new ClientUser { ClientId = client1.Id, UserId = user1.Id };
            var clientUser2 = new ClientUser { ClientId = client2.Id, UserId = user2.Id };
            context.ClientUsers.AddRange(clientUser1, clientUser2);

            var projectUser1 = new ProjectUser { ProjectId = project1.Id, UserId = user1.Id };
            var projectUser2 = new ProjectUser { ProjectId = project2.Id, UserId = user2.Id };
            context.ProjectUsers.AddRange(projectUser1, projectUser2);

            var userRole1 = new UserRole { UserId = user1.Id, RoleId = role1.Id };
            var userRole2 = new UserRole { UserId = user2.Id, RoleId = role2.Id };
            context.UserRoles.AddRange(userRole1, userRole2);

            context.SaveChanges();
        }


        public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
        {
            public TestAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
            {
            }
            protected override Task<AuthenticateResult> HandleAuthenticateAsync()
            {
                var identity = new ClaimsIdentity(Array.Empty<Claim>(), "Test");
                var principal = new ClaimsPrincipal(identity);
                var ticket = new AuthenticationTicket(principal, "TestScheme");

                var result = AuthenticateResult.Success(ticket);

                return Task.FromResult(result);
            }

        }
        public class AllowAnonymousAuthorizationHandler : IAuthorizationHandler
        {
            public Task HandleAsync(AuthorizationHandlerContext context)
            {
                foreach (var requirement in context.PendingRequirements.ToList())
                {
                    context.Succeed(requirement);
                }
                return Task.CompletedTask;
            }
        }
    }
}
