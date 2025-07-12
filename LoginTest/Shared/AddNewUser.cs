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

            var testUser = new User
            {
                Username = isAdmin ? "testadmin" : "testuser",
                Email = isAdmin ? "testadmin@example.com" : "testuser@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Pa$$w0rd!"),
                Name = "testname",
                Surname = "testsurname",
                RoleId = isAdmin ? (int)RoleEnum.Admin : (int)RoleEnum.Employee,
            };

            context.User.Add(testUser);
            await context.SaveChangesAsync();

            return testUser;
        }
    }
}
