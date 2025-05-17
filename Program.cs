using StoreAPI.Models;
using StoreAPI.Models.Contexts;
using StoreAPI.Repositories;
using StoreAPI.Repositories.Interfaces;
using StoreAPI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme) // for authentication
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration.GetValue<string>("Authentication:Issuer"),
            ValidateAudience = true,
            ValidAudience = builder.Configuration.GetValue<string>("Authentication:Audience"),
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetValue<string>("Authentication:SecretKey")))
        };
    });

builder.Services.AddAuthorization(); // for authentication

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<StoreContext>(o => o.UseSqlServer(connectionString));

//
// Services
//
builder.Services.AddScoped<DateSaleSumsViewService>();
builder.Services.AddScoped<DatePurchaseSumsViewService>();
builder.Services.AddScoped<ProductSaleSumsAndProfitViewService>();
builder.Services.AddScoped<SupplierPurchaseSumsViewService>(); 
builder.Services.AddScoped<ProductPurchaseSumsViewService>();
builder.Services.AddScoped<CitySaleSumsViewService>();
builder.Services.AddScoped<CustomerSaleSumsViewService>();
builder.Services.AddScoped<ProductTypeSaleSumsAndProfitViewService>(); 
builder.Services.AddScoped<BrandSaleSumsAndProfitViewService>();
builder.Services.AddScoped<DayOfWeekSaleSumsViewService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddSingleton<TokenService>();

//
// Repositories
//
builder.Services.AddScoped<IDateSaleSumsViewRepository, DateSaleSumsViewRepository>();
builder.Services.AddScoped<IDatePurchaseSumsViewRepository, DatePurchaseSumsViewRepository>();
builder.Services.AddScoped<IProductSaleSumsAndProfitViewRepository, ProductSaleSumsAndProfitViewRepository>();
builder.Services.AddScoped<ISupplierPurchaseSumsViewRepository, SupplierPurchaseSumsViewRepository>();
builder.Services.AddScoped<IProductPurchaseSumsViewRepository, ProductPurchaseSumsViewRepository>();
builder.Services.AddScoped<ICitySaleSumsViewRepository, CitySaleSumsViewRepository>();
builder.Services.AddScoped<ICustomerSaleSumsViewRepository, CustomerSaleSumsViewRepository>();
builder.Services.AddScoped<IProductTypeSaleSumsAndProfitViewRepository, ProductTypeSaleSumsAndProfitViewRepository>();
builder.Services.AddScoped<IBrandSaleSumsAndProfitViewRepository, BrandSaleSumsAndProfitViewRepository>();
builder.Services.AddScoped<IDayOfWeekSaleSumsViewRepository, DayOfWeekSaleSumsViewRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddCors(options =>//I used this to avoid the CORS errors, also check allow-cross-origin-credentials to true (in extension or in api somehow)
{
    options.AddPolicy("CorsPolicy",
            builder => builder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("CorsPolicy");

app.UseAuthentication(); // for authentication

app.UseAuthorization();

app.MapControllers();

app.Run();
