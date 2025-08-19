
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

        public async Task<User> CreateTestUserAsync(string prefix, bool isAdmin = false, string name = null, string surname = null)
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
                email = prefix+CreateRandomEmail();

                // Check if a user with the same username or email already exists
                userExists = await context.User.AnyAsync(u => u.Username == username || u.Email == email);
            }

            var testUser = new User
            {
                Username = username,
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Pa$$w0rd!"),
                Name = name == null ? CreateRandomText() : name,
                Surname = surname == null ? CreateRandomText() : surname,
                RoleId = isAdmin ? (int)RoleEnum.Admin : (int)RoleEnum.Employee,
            };

            context.User.Add(testUser);
            await context.SaveChangesAsync();

            return testUser;
        }

        public async Task<Company> CreateTestCompanyAsync(string companyNamePrefix = null)
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<StoreContext>();

            var name = companyNamePrefix ?? string.Empty;
            var address = string.Empty;
            var phone = string.Empty;
            var companyExists = true;

            while (companyExists)
            {
                name += CreateRandomText();
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

        public string CreateRandomText(int? sizeLimit = null)
        {
            var text = $"randomtext{Guid.NewGuid().ToString()}";
            return sizeLimit.HasValue && text.Length > sizeLimit.Value ? text.Substring(0, sizeLimit.Value) : text;
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

        public void CleanUp(string prefix)
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<StoreContext>();

            var testCompanies = context.Company
                .Where(c => c.Name.StartsWith(prefix))
                .ToList();

            var testCompanyIds = testCompanies.Select(c => c.Id).ToList();
            var testProducts = context.Product
                .Where(p => testCompanyIds.Contains(p.CompanyId))
                .ToList();

            var testProductIds = testProducts.Select(c => c.Id).ToList();
            var testProductSales = context.ProductSale
                .Where(p => testProductIds.Contains(p.ProductId))
                .ToList();

            if (testProductSales.Any())
            {
                context.ProductSale.RemoveRange(testProductSales);
            }

            context.SaveChanges();

            if (testProducts.Any())
            {
                context.Product.RemoveRange(testProducts);
            }

            context.SaveChanges();

            if (testCompanies.Any())
            {
                context.Company.RemoveRange(testCompanies);
            }

            context.SaveChanges();

            var testUsers = context.User
                .Where(u => u.Email.StartsWith(prefix))
                .ToList();

            if (testUsers.Any())
            {
                context.User.RemoveRange(testUsers);
            }

            context.SaveChanges();
        }
    }
}
