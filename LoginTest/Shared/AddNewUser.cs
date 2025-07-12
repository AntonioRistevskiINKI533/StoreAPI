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

namespace StoreAPI.IntegrationTests.Shared
{
    internal class Helper
    {
        private async Task<User> CreateTestUserAsync(CustomWebApplicationFactory _factory)
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<StoreContext>();

            var testUser = new User
            {
                Username = "testuser",
                Email = "testuser@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Pa$$w0rd!"),
                //CreatedAt = DateTime.UtcNow
                Name = "testname",
                Surname = "testsurname",
                RoleId = (int)RoleEnum.Employee,
            };

            context.User.Add(testUser);
            await context.SaveChangesAsync();

            return testUser;
        }
    }
}
