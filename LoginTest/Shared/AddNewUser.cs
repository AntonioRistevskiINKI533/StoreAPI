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
                email = $"testuser+{guid}@example.com";

                // Check if a user with the same username or email already exists
                userExists = await context.User.AnyAsync(u => u.Username == username || u.Email == email);
            }

            var testUser = new User
            {
                Username = username,
                Email = email,
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
