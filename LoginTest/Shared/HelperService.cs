using StoreAPI.Enums;
using StoreAPI.Models.Contexts;
using StoreAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StoreAPI.Services;
using Microsoft.EntityFrameworkCore;

namespace StoreAPI.IntegrationTests.Shared
{
    public class HelperService
    {
        private readonly CustomWebApplicationFactory _factory;

        public HelperService(CustomWebApplicationFactory factory)
        {
            _factory = factory;
        }

        public async Task<User> CreateTestUserAsync(bool isAdmin = false)
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<StoreContext>();

            var username = string.Empty;
            var email = string.Empty;
            var userExists = true;

            while(userExists)
            {
                var guid = Guid.NewGuid();
                username = $"testuser{guid.ToString().Substring(0, 12)}";
                email = CreateRandomEmail();

                // Check if a user with the same username or email already exists
                userExists = await context.User.AnyAsync(u => u.Username == username || u.Email == email);
            }

            var testUser = new User
            {
                Username = username,
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Pa$$w0rd!"),
                Name = CreateRandomText(),
                Surname = CreateRandomText(),
                RoleId = isAdmin ? (int)RoleEnum.Admin : (int)RoleEnum.Employee,
            };

            context.User.Add(testUser);
            await context.SaveChangesAsync();

            return testUser;
        }

        public async Task<Company> CreateTestCompanyAsync()
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<StoreContext>();

            var name = string.Empty;
            var address = string.Empty;
            var phone = string.Empty;
            var companyExists = true;

            while (companyExists)
            {
                name = CreateRandomText();
                address = CreateRandomText();
                phone = CreateRandomPhoneNumber();

                // Check if a company with the same name, address or phone already exists
                companyExists = await context.Company.AnyAsync(u => u.Name == name || u.Address == address || u.Phone == phone);
            }

            var testCompany = new Company
            {
                Name = name,
                Address = address,
                Phone = phone
            };

            context.Company.Add(testCompany);
            await context.SaveChangesAsync();

            return testCompany;
        }

        public async Task<Product> CreateTestProductAsync(int companyId)
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<StoreContext>();

            var registrationNumber = string.Empty;
            var name = string.Empty;
            var productExists = true;

            while (productExists)
            {
                registrationNumber = new Random().Next(1000000, 9999999).ToString();
                name = CreateRandomText();

                // Check if a product with the same name, registrationNumber already exists
                productExists = await context.Product.AnyAsync(u => u.Name == name || u.RegistrationNumber == registrationNumber);
            }

            var testProduct = new Product
            {
                RegistrationNumber = registrationNumber,
                Name = name,
                CompanyId = companyId,
                Price = 10
            };

            context.Product.Add(testProduct);
            await context.SaveChangesAsync();

            return testProduct;
        }

        public async Task<ProductSale> CreateTestProductSaleAsync(int productId, DateTime? date = null)
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<StoreContext>();

            var testProductSale = new ProductSale
            {
                ProductId = productId,
                SoldAmount = new Random().Next(1, 99),
                PricePerUnit = CreateRandomPrice(),
                Date = date ?? DateTime.UtcNow
            };

            context.ProductSale.Add(testProductSale);
            await context.SaveChangesAsync();

            return testProductSale;
        }

        public string CreateRandomPhoneNumber()
        {
            return $"+389{Random.Shared.Next(0, 10)}{Random.Shared.Next(10000000, 99999999)}";
        }

        public string CreateRandomText()
        {
            return $"randomtext{Guid.NewGuid().ToString()}";
        }

        public string CreateRandomEmail()
        {
            return $"{Guid.NewGuid().ToString()}@gmail.com";
        }

        public decimal CreateRandomPrice()
        {
            return Math.Round(new Random().Next(1000, 100000) / 100m, 2);
        }

        public int CreateRandomNumber()
        {
            return Random.Shared.Next(1, 20);
        }
    }
}
