using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using StoreAPI.Services;
using System.Net.Http;
using StoreAPI.Models.Contexts;
using StoreAPI.Models.Requests;
using StoreAPI.IntegrationTests.Shared;
using StoreAPI.Enums;

namespace StoreAPI.IntegrationTests.UserController
{
    public class UpdateUserIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory _factory;
        private readonly HelperService _helperService;

        public UpdateUserIntegrationTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
            _helperService = new HelperService(_factory);
        }

        [Fact]
        public async Task UpdateUser_WithAdminTokenAndValidData_ShouldReturnOk()
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
            var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

            var adminUser = await _helperService.CreateTestUserAsync(true);

            var testUser = await _helperService.CreateTestUserAsync();

            var token = tokenService.GenerateToken(adminUser.Id, ((RoleEnum)adminUser.RoleId).ToString());

            var updateRequest = new UpdateUserRequest
            {
                Id = testUser.Id,
                Username = _helperService.CreateRandomText(20),
                Email = _helperService.CreateRandomEmail(),
                Name = _helperService.CreateRandomText(),
                Surname = _helperService.CreateRandomText(),
                RoleId = (int)RoleEnum.Employee
            };

            var request = new HttpRequestMessage(HttpMethod.Put, "/User/UpdateUser")
            {
                Content = JsonContent.Create(updateRequest)
            };
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _client.SendAsync(request);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

            var updatedUser = await context.User.FindAsync(testUser.Id);
            updatedUser.Should().NotBeNull();
            updatedUser.Username.Should().Be(updateRequest.Username);
            updatedUser.Email.Should().Be(updateRequest.Email);
            updatedUser.Name.Should().Be(updateRequest.Name);
            updatedUser.Surname.Should().Be(updateRequest.Surname);
            updatedUser.RoleId.Should().Be(updateRequest.RoleId);

            //Clean up
            context.User.Remove(adminUser);
            context.User.Remove(updatedUser);
            await context.SaveChangesAsync();
        }

        [Fact]
        public async Task UpdateUser_WithoutToken_ShouldReturnUnauthorized()
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<StoreContext>();

            var testUser = await _helperService.CreateTestUserAsync();

            var updateRequest = new UpdateUserRequest
            {
                Id = testUser.Id,
                Username = _helperService.CreateRandomText(20),
                Email = _helperService.CreateRandomEmail(),
                Name = _helperService.CreateRandomText(),
                Surname = _helperService.CreateRandomText(),
                RoleId = (int)RoleEnum.Employee
            };

            var response = await _client.PutAsJsonAsync("/User/UpdateUser", updateRequest);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);

            //Clean up
            context.User.Remove(testUser);
            await context.SaveChangesAsync();
        }

        [Fact]
        public async Task UpdateUser_WithEmployeeToken_ShouldReturnForbidden()
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
            var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

            var employeeUser = await _helperService.CreateTestUserAsync();

            var testUser = await _helperService.CreateTestUserAsync();

            var token = tokenService.GenerateToken(employeeUser.Id, ((RoleEnum)employeeUser.RoleId).ToString());

            var updateRequest = new UpdateUserRequest
            {
                Id = testUser.Id,
                Username = _helperService.CreateRandomText(20),
                Email = _helperService.CreateRandomEmail(),
                Name = _helperService.CreateRandomText(),
                Surname = _helperService.CreateRandomText(),
                RoleId = (int)RoleEnum.Employee
            };

            var request = new HttpRequestMessage(HttpMethod.Put, "/User/UpdateUser")
            {
                Content = JsonContent.Create(updateRequest)
            };
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _client.SendAsync(request);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Forbidden);

            //Clean up
            context.User.Remove(employeeUser);
            context.User.Remove(testUser);
            await context.SaveChangesAsync();
        }

        [Fact]
        public async Task UpdateUser_WithNonExistentUserId_ShouldReturnNotFound()
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
            var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

            var adminUser = await _helperService.CreateTestUserAsync(true);

            var testUser = await _helperService.CreateTestUserAsync();

            context.User.Remove(testUser);
            await context.SaveChangesAsync();

            var token = tokenService.GenerateToken(adminUser.Id, ((RoleEnum)adminUser.RoleId).ToString());

            var updateRequest = new UpdateUserRequest
            {
                Id = testUser.Id,
                Username = _helperService.CreateRandomText(20),
                Email = _helperService.CreateRandomEmail(),
                Name = _helperService.CreateRandomText(),
                Surname = _helperService.CreateRandomText(),
                RoleId = (int)RoleEnum.Employee
            };

            var request = new HttpRequestMessage(HttpMethod.Put, "/User/UpdateUser")
            {
                Content = JsonContent.Create(updateRequest)
            };
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _client.SendAsync(request);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);

            var content = await response.Content.ReadAsStringAsync();
            content.Should().Be("User does not exist");

            //Clean up
            context.User.Remove(adminUser);
            await context.SaveChangesAsync();
        }

        [Fact]
        public async Task UpdateUser_WithExistingUsername_ShouldReturnConflict()
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
            var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

            var adminUser = await _helperService.CreateTestUserAsync(true);

            var testUser = await _helperService.CreateTestUserAsync();

            var anotherUser = await _helperService.CreateTestUserAsync();

            var token = tokenService.GenerateToken(adminUser.Id, ((RoleEnum)adminUser.RoleId).ToString());

            var updateRequest = new UpdateUserRequest
            {
                Id = testUser.Id,
                Username = anotherUser.Username,
                Email = _helperService.CreateRandomEmail(),
                Name = _helperService.CreateRandomText(),
                Surname = _helperService.CreateRandomText(),
                RoleId = (int)RoleEnum.Employee
            };

            var request = new HttpRequestMessage(HttpMethod.Put, "/User/UpdateUser")
            {
                Content = JsonContent.Create(updateRequest)
            };
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _client.SendAsync(request);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Conflict);

            var content = await response.Content.ReadAsStringAsync();
            content.Should().Be("User with same username already exists");

            //Clean up
            context.User.Remove(adminUser);
            context.User.Remove(testUser);
            context.User.Remove(anotherUser);
            await context.SaveChangesAsync();
        }

        [Fact]
        public async Task UpdateUser_WithExistingEmail_ShouldReturnConflict()
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
            var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

            var adminUser = await _helperService.CreateTestUserAsync(true);

            var testUser = await _helperService.CreateTestUserAsync();

            var anotherUser = await _helperService.CreateTestUserAsync();

            var token = tokenService.GenerateToken(adminUser.Id, ((RoleEnum)adminUser.RoleId).ToString());

            var updateRequest = new UpdateUserRequest
            {
                Id = testUser.Id,
                Username = _helperService.CreateRandomText(20),
                Email = anotherUser.Email,
                Name = _helperService.CreateRandomText(),
                Surname = _helperService.CreateRandomText(),
                RoleId = (int)RoleEnum.Employee
            };

            var request = new HttpRequestMessage(HttpMethod.Put, "/User/UpdateUser")
            {
                Content = JsonContent.Create(updateRequest)
            };
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _client.SendAsync(request);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Conflict);

            var content = await response.Content.ReadAsStringAsync();
            content.Should().Be("User with same email already exists");

            //Clean up
            context.User.Remove(adminUser);
            context.User.Remove(testUser);
            context.User.Remove(anotherUser);
            await context.SaveChangesAsync();
        }

        [Fact]
        public async Task UpdateUser_WithInvalidEmailFormat_ShouldReturnBadRequest()
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
            var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

            var adminUser = await _helperService.CreateTestUserAsync(true);

            var testUser = await _helperService.CreateTestUserAsync();

            var token = tokenService.GenerateToken(adminUser.Id, ((RoleEnum)adminUser.RoleId).ToString());

            var updateRequest = new UpdateUserRequest
            {
                Id = testUser.Id,
                Username = _helperService.CreateRandomText(20),
                Email = "invalid-email-format", //invalid email
                Name = _helperService.CreateRandomText(),
                Surname = _helperService.CreateRandomText(),
                RoleId = (int)RoleEnum.Employee
            };

            var request = new HttpRequestMessage(HttpMethod.Put, "/User/UpdateUser")
            {
                Content = JsonContent.Create(updateRequest)
            };
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _client.SendAsync(request);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);

            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("Invalid email");

            //Clean up
            context.User.Remove(adminUser);
            context.User.Remove(testUser);
            await context.SaveChangesAsync();
        }

        [Fact]
        public async Task UpdateUser_WithUsernameTooShort_ShouldReturnBadRequest()
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
            var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

            var adminUser = await _helperService.CreateTestUserAsync(true);

            var testUser = await _helperService.CreateTestUserAsync();

            var token = tokenService.GenerateToken(adminUser.Id, ((RoleEnum)adminUser.RoleId).ToString());

            var updateRequest = new UpdateUserRequest
            {
                Id = testUser.Id,
                Username = "abc", //too short (min length = 5)
                Email = _helperService.CreateRandomEmail(),
                Name = _helperService.CreateRandomText(),
                Surname = _helperService.CreateRandomText(),
                RoleId = (int)RoleEnum.Employee
            };

            var request = new HttpRequestMessage(HttpMethod.Put, "/User/UpdateUser")
            {
                Content = JsonContent.Create(updateRequest)
            };
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _client.SendAsync(request);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);

            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("Username must be between 5 and 20 characters");

            //Clean up
            context.User.Remove(adminUser);
            context.User.Remove(testUser);
            await context.SaveChangesAsync();
        }

        [Fact]
        public async Task UpdateUser_WithUsernameTooLong_ShouldReturnBadRequest()
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
            var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

            var adminUser = await _helperService.CreateTestUserAsync(true);

            var testUser = await _helperService.CreateTestUserAsync();

            var token = tokenService.GenerateToken(adminUser.Id, ((RoleEnum)adminUser.RoleId).ToString());

            var updateRequest = new UpdateUserRequest
            {
                Id = testUser.Id,
                Username = new string('A', 21), //too long
                Email = _helperService.CreateRandomEmail(),
                Name = _helperService.CreateRandomText(),
                Surname = _helperService.CreateRandomText(),
                RoleId = (int)RoleEnum.Employee
            };

            var request = new HttpRequestMessage(HttpMethod.Put, "/User/UpdateUser")
            {
                Content = JsonContent.Create(updateRequest)
            };
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _client.SendAsync(request);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);

            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("Username must be between 5 and 20 characters");

            //Clean up
            context.User.Remove(adminUser);
            context.User.Remove(testUser);
            await context.SaveChangesAsync();
        }

        [Fact]
        public async Task UpdateUser_WithNameTooShort_ShouldReturnBadRequest()
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
            var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

            var adminUser = await _helperService.CreateTestUserAsync(true);

            var testUser = await _helperService.CreateTestUserAsync();

            var token = tokenService.GenerateToken(adminUser.Id, ((RoleEnum)adminUser.RoleId).ToString());

            var updateRequest = new UpdateUserRequest
            {
                Id = testUser.Id,
                Username = _helperService.CreateRandomText(20),
                Email = _helperService.CreateRandomEmail(),
                Name = "",
                Surname = _helperService.CreateRandomText(),
                RoleId = (int)RoleEnum.Employee
            };

            var request = new HttpRequestMessage(HttpMethod.Put, "/User/UpdateUser")
            {
                Content = JsonContent.Create(updateRequest)
            };
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _client.SendAsync(request);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);

            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("Name must be between 1 and 100 characters");

            //Clean up
            context.User.Remove(adminUser);
            context.User.Remove(testUser);
            await context.SaveChangesAsync();
        }

        [Fact]
        public async Task UpdateUser_WithNameTooLong_ShouldReturnBadRequest()
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
            var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

            var adminUser = await _helperService.CreateTestUserAsync(true);

            var testUser = await _helperService.CreateTestUserAsync();

            var token = tokenService.GenerateToken(adminUser.Id, ((RoleEnum)adminUser.RoleId).ToString());

            var updateRequest = new UpdateUserRequest
            {
                Id = testUser.Id,
                Username = _helperService.CreateRandomText(20),
                Email = _helperService.CreateRandomEmail(),
                Name = new string('A', 101), // too long (max length = 100)
                Surname = _helperService.CreateRandomText(),
                RoleId = (int)RoleEnum.Employee
            };

            var request = new HttpRequestMessage(HttpMethod.Put, "/User/UpdateUser")
            {
                Content = JsonContent.Create(updateRequest)
            };
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _client.SendAsync(request);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);

            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("Name must be between 1 and 100 characters");

            //Clean up
            context.User.Remove(adminUser);
            context.User.Remove(testUser);
            await context.SaveChangesAsync();
        }

        [Fact]
        public async Task UpdateUser_WithSurnameTooShort_ShouldReturnBadRequest()
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
            var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

            var adminUser = await _helperService.CreateTestUserAsync(true);

            var testUser = await _helperService.CreateTestUserAsync();

            var token = tokenService.GenerateToken(adminUser.Id, ((RoleEnum)adminUser.RoleId).ToString());

            var updateRequest = new UpdateUserRequest
            {
                Id = testUser.Id,
                Username = _helperService.CreateRandomText(20),
                Email = _helperService.CreateRandomEmail(),
                Name = _helperService.CreateRandomText(),
                Surname = "", //too short
                RoleId = (int)RoleEnum.Employee
            };

            var request = new HttpRequestMessage(HttpMethod.Put, "/User/UpdateUser")
            {
                Content = JsonContent.Create(updateRequest)
            };
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _client.SendAsync(request);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);

            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("Surname must be between 1 and 100 characters");

            //Clean up
            context.User.Remove(adminUser);
            context.User.Remove(testUser);
            await context.SaveChangesAsync();
        }

        [Fact]
        public async Task UpdateUser_WithSurnameTooLong_ShouldReturnBadRequest()
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
            var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

            var adminUser = await _helperService.CreateTestUserAsync(true);

            var testUser = await _helperService.CreateTestUserAsync();

            var token = tokenService.GenerateToken(adminUser.Id, ((RoleEnum)adminUser.RoleId).ToString());

            var updateRequest = new UpdateUserRequest
            {
                Id = testUser.Id,
                Username = _helperService.CreateRandomText(20),
                Email = _helperService.CreateRandomEmail(),
                Name = _helperService.CreateRandomText(),
                Surname = new string('A', 101), //too long
                RoleId = (int)RoleEnum.Employee
            };

            var request = new HttpRequestMessage(HttpMethod.Put, "/User/UpdateUser")
            {
                Content = JsonContent.Create(updateRequest)
            };
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _client.SendAsync(request);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);

            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("Surname must be between 1 and 100 characters");

            //Clean up
            context.User.Remove(adminUser);
            context.User.Remove(testUser);
            await context.SaveChangesAsync();
        }

        [Fact]
        public async Task UpdateUser_WithNonExistentRoleId_ShouldReturnNotFound()
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
            var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

            var adminUser = await _helperService.CreateTestUserAsync(true);

            var testUser = await _helperService.CreateTestUserAsync();

            var token = tokenService.GenerateToken(adminUser.Id, ((RoleEnum)adminUser.RoleId).ToString());

            var updateRequest = new UpdateUserRequest
            {
                Id = testUser.Id,
                Username = _helperService.CreateRandomText(20),
                Email = _helperService.CreateRandomEmail(),
                Name = _helperService.CreateRandomText(),
                Surname = _helperService.CreateRandomText(),
                RoleId = 9999 //Non-existent role ID
            };

            var request = new HttpRequestMessage(HttpMethod.Put, "/User/UpdateUser")
            {
                Content = JsonContent.Create(updateRequest)
            };
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _client.SendAsync(request);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);

            var content = await response.Content.ReadAsStringAsync();
            content.Should().Be("Role does not exist");

            //Clean up
            context.User.Remove(adminUser);
            context.User.Remove(testUser);
            await context.SaveChangesAsync();
        }
    }
}