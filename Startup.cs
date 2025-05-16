/*using StoreAPI.Models;
using StoreAPI.Repositories;
using StoreAPI.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
//using Microsoft.TodoApi.Nodels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StoreAPI
{
    public class Startup
    {

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSwaggerGen(c => 
            {
               c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "StoreAPI", Version = "v1" });
            });

            var connectionString = Configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<StoreContext>(o => o.UseSqlServer(connectionString));

            //
            // Services
            //
            services.AddScoped<DateSaleSumsViewService>();

            //
            // Repositories
            //
            services.AddScoped<IDateSaleSumsViewRepository, DateSaleSumsViewRepository>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c => 
            {
               c.SwaggerEndpoint("/swagger/v1/swagger.json", "BookAPI V");
               c.RoutePrefix ="";
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }


            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            app.UseStaticFiles();



        }
            // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        

     
    }
}
*/