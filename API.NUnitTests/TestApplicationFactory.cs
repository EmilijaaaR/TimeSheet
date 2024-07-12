using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Infrastructure.Data;
using Microsoft.Extensions.Configuration;

namespace API.NUnitTests
{
    public class TestApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.AddJsonFile("testhost.deps.json", optional: true, reloadOnChange: true);
                })
                .ConfigureServices(services =>
                {
                    services.AddDbContext<TimeSheetDbContext>(options =>
                    {
                        options.UseInMemoryDatabase("TestDatabase");
                    });
                });
        }
    }
}
