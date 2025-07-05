using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using StoreAPI;
using StoreAPI.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
// using VDS.RDF.Query.Algebra;
using StoreAPI.Models;
using Microsoft.AspNetCore.TestHost;
using StoreAPI.Models.Contexts;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using StoreAPI.Repositories.Interfaces;
using StoreAPI.Repositories;



public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    //private readonly MongoDbRunner _mongoRunner = MongoDbRunner.Start();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((context, config) =>
        {
            //var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

            var inMemorySettings = new Dictionary<string, string?>
            {
                ["StoreDatabaseSettings:ConnectionString"] = "data source=DESKTOP-41L0M4T\\SQLEXPRESS; initial catalog=StoreDB; integrated security=True; MultipleActiveResultSets=True; Encrypt=False;"
            };


            config.AddInMemoryCollection(inMemorySettings);
        });

        builder.ConfigureLogging(logging =>
        {
            logging.ClearProviders();
            logging.AddConsole(); // Send logs to output
        });

        builder.UseEnvironment("Development");

        builder.ConfigureTestServices(services =>
        {
            // Override only what you need
            /*            services.Configure<SemanticDatabaseSettings>(options =>
                        {
                            options.ConnectionString = _mongoRunner.ConnectionString;
                            options.DatabaseName = "TestDb";
                            options.RdfCollectionName = "RdfTest";
                        });*/

            // Remove the real DbContext registration
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<StoreContext>));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // Add an in-memory database for testing
            /*            services.AddDbContext<StoreContext>(options =>
                        {
                            options.UseInMemoryDatabase("InMemoryTestDb");
                        });*/
            services.AddDbContext<StoreContext>(options =>
            {
                options.UseSqlServer("data source=DESKTOP-41L0M4T\\SQLEXPRESS; initial catalog=StoreDB; integrated security=True; MultipleActiveResultSets=True; Encrypt=False;");
            });

            services.AddScoped<ICompanyRepository, CompanyRepository>();
            services.AddScoped<IProductSaleRepository, ProductSaleRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IUserRepository, UserRepository>();

            services.AddScoped<CompanyService>();
            services.AddScoped<ProductSaleService>();
            services.AddScoped<ProductService>();
            services.AddScoped<RoleService>();
            services.AddScoped<UserService>();
            services.AddScoped<TokenService>();

            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<StoreContext>();
            db.Database.Migrate(); // Applies migrations at startup
        });

    }

/*    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing)
        {
            _mongoRunner.Dispose();
        }
    }*/
}
