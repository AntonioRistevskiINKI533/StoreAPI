using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using StoreAPI.Services;
using Microsoft.AspNetCore.Hosting;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.TestHost;
using StoreAPI.Models.Contexts;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using StoreAPI.Repositories.Interfaces;
using StoreAPI.Repositories;


namespace StoreAPI.IntegrationTests.Shared
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<StoreContext>));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                services.AddDbContext<StoreContext>((sp, options) =>
                {
                    var cfg = sp.GetRequiredService<IConfiguration>();
                    var cs = cfg.GetConnectionString("TestConnection");
                    options.UseSqlServer(cs);
                });
            });

            builder.ConfigureLogging(logging =>
            {
                logging.ClearProviders(); 
                logging.AddConsole();
            });
        }
    }
}