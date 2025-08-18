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
                    //options.UseInMemoryDatabase("TestDb"); could use this for inMemory tests
                });

                //could be used if i use dummy services and i need to mock repositories
/*              services.AddScoped<ICompanyRepository, CompanyRepository>();
                services.AddScoped<IProductSaleRepository, ProductSaleRepository>();
                services.AddScoped<IProductRepository, ProductRepository>();
                services.AddScoped<IRoleRepository, RoleRepository>();
                services.AddScoped<IUserRepository, UserRepository>();

                services.AddScoped<CompanyService>();
                services.AddScoped<ProductSaleService>();
                services.AddScoped<ProductService>();
                services.AddScoped<RoleService>();
                services.AddScoped<UserService>();
                services.AddScoped<TokenService>();*/

                /*var sp = services.BuildServiceProvider();
                using var scope = sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<StoreContext>();
                db.Database.Migrate();*/ // Applies migrations at startup
            });

            //clear default logging providers and send console logs to output
            builder.ConfigureLogging(logging =>
            {
                logging.ClearProviders(); 
                logging.AddConsole();
            });
        }
    }
}