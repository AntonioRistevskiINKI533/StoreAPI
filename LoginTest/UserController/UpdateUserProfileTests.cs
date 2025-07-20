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
    public class UpdateUserProfileIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory _factory;
        private readonly HelperService _helperService;

        public UpdateUserProfileIntegrationTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
            _helperService = new HelperService(_factory);
        }

        [Fact]
        public async Task UpdateUserProfile_WithValidTokenAndData_ShouldReturnOk()
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
            var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

            var testUser = await _helperService.CreateTestUserAsync();

            var token = tokenService.GenerateToken(testUser.Id, ((RoleEnum)testUser.RoleId).ToString());

            var updateRequest = new UpdateUserProfileRequest
            {
                Username = "updatedusername",
                Email = _helperService.CreateRandomEmail(),
                Name = _helperService.CreateRandomText(),
                Surname = _helperService.CreateRandomText()
            };

            var request = new HttpRequestMessage(HttpMethod.Put, "/User/UpdateUserProfile")
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

            //Clean up
            context.User.Remove(updatedUser);
            await context.SaveChangesAsync();
        }

        [Fact]
        public async Task UpdateUserProfile_WithNonExistentUserId_ShouldReturnNotFound()
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
            var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

            var testUser = await _helperService.CreateTestUserAsync();

            context.User.Remove(testUser);
            await context.SaveChangesAsync();

            var token = tokenService.GenerateToken(testUser.Id, ((RoleEnum)testUser.RoleId).ToString());

            var updateRequest = new UpdateUserProfileRequest
            {
                Username = "updatedusername",
                Email = _helperService.CreateRandomEmail(),
                Name = _helperService.CreateRandomText(),
                Surname = _helperService.CreateRandomText()
            };

            var request = new HttpRequestMessage(HttpMethod.Put, "/User/UpdateUserProfile")
            {
                Content = JsonContent.Create(updateRequest)
            };
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _client.SendAsync(request);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);

            var content = await response.Content.ReadAsStringAsync();
            content.Should().Be("User does not exist");
        }

        [Fact]
        public async Task UpdateUserProfile_WithoutToken_ShouldReturnUnauthorized()
        {
            var updateRequest = new UpdateUserProfileRequest
            {
                Username = "updatedusername",
                Email = _helperService.CreateRandomEmail(),
                Name = _helperService.CreateRandomText(),
                Surname = _helperService.CreateRandomText()
            };

            var response = await _client.PutAsJsonAsync("/User/UpdateUserProfile", updateRequest);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task UpdateUserProfile_WithExistingUsername_ShouldReturnConflict()
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
            var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

            var testUser = await _helperService.CreateTestUserAsync();

            var anotherUser = await _helperService.CreateTestUserAsync();

            var token = tokenService.GenerateToken(testUser.Id, ((RoleEnum)testUser.RoleId).ToString());

            var updateRequest = new UpdateUserProfileRequest
            {
                Username = anotherUser.Username,
                Email = _helperService.CreateRandomEmail(),
                Name = _helperService.CreateRandomText(),
                Surname = _helperService.CreateRandomText()
            };

            var request = new HttpRequestMessage(HttpMethod.Put, "/User/UpdateUserProfile")
            {
                Content = JsonContent.Create(updateRequest)
            };
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _client.SendAsync(request);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Conflict);

            var content = await response.Content.ReadAsStringAsync();
            content.Should().Be("User with same username already exists");

            //Clean up
            context.User.Remove(testUser);
            context.User.Remove(anotherUser);
            await context.SaveChangesAsync();
        }

        [Fact]
        public async Task UpdateUserProfile_WithExistingEmail_ShouldReturnConflict()
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
            var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

            var testUser = await _helperService.CreateTestUserAsync();

            var anotherUser = await _helperService.CreateTestUserAsync();

            var token = tokenService.GenerateToken(testUser.Id, ((RoleEnum)testUser.RoleId).ToString());

            var updateRequest = new UpdateUserProfileRequest
            {
                Username = "updatedusername",
                Email = anotherUser.Email,
                Name = _helperService.CreateRandomText(),
                Surname = _helperService.CreateRandomText()
            };

            var request = new HttpRequestMessage(HttpMethod.Put, "/User/UpdateUserProfile")
            {
                Content = JsonContent.Create(updateRequest)
            };
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _client.SendAsync(request);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Conflict);

            var content = await response.Content.ReadAsStringAsync();
            content.Should().Be("User with same email already exists");

            //Clean up
            context.User.Remove(testUser);
            context.User.Remove(anotherUser);
            await context.SaveChangesAsync();
        }

        [Fact]
        public async Task UpdateUserProfile_WithInvalidEmailFormat_ShouldReturnBadRequest()
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
            var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

            var testUser = await _helperService.CreateTestUserAsync();

            var token = tokenService.GenerateToken(testUser.Id, ((RoleEnum)testUser.RoleId).ToString());

            var updateRequest = new UpdateUserProfileRequest
            {
                Username = "updatedusername",
                Email = "invalid-email-format", //invalid email
                Name = _helperService.CreateRandomText(),
                Surname = _helperService.CreateRandomText()
            };

            var request = new HttpRequestMessage(HttpMethod.Put, "/User/UpdateUserProfile")
            {
                Content = JsonContent.Create(updateRequest)
            };
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _client.SendAsync(request);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);

            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("Invalid email");

            //Clean up
            context.User.Remove(testUser);
            await context.SaveChangesAsync();
        }

        [Fact]
        public async Task UpdateUserProfile_WithUsernameTooShort_ShouldReturnBadRequest()
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
            var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

            var testUser = await _helperService.CreateTestUserAsync();
            var token = tokenService.GenerateToken(testUser.Id, ((RoleEnum)testUser.RoleId).ToString());

            var updateRequest = new UpdateUserProfileRequest
            {
                Username = "abc", //too short (min length = 5)
                Email = _helperService.CreateRandomEmail(),
                Name = _helperService.CreateRandomText(),
                Surname = _helperService.CreateRandomText()
            };

            var request = new HttpRequestMessage(HttpMethod.Put, "/User/UpdateUserProfile")
            {
                Content = JsonContent.Create(updateRequest)
            };
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _client.SendAsync(request);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);

            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("Username must be between 5 and 20 characters");

            //Clean up
            context.User.Remove(testUser);
            await context.SaveChangesAsync();
        }

        [Fact]
        public async Task UpdateUserProfile_WithUsernameTooLong_ShouldReturnBadRequest()
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
            var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

            var testUser = await _helperService.CreateTestUserAsync();
            var token = tokenService.GenerateToken(testUser.Id, ((RoleEnum)testUser.RoleId).ToString());

            var updateRequest = new UpdateUserProfileRequest
            {
                Username = new string('A', 21), //too long
                Email = _helperService.CreateRandomEmail(),
                Name = _helperService.CreateRandomText(),
                Surname = _helperService.CreateRandomText()
            };

            var request = new HttpRequestMessage(HttpMethod.Put, "/User/UpdateUserProfile")
            {
                Content = JsonContent.Create(updateRequest)
            };
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _client.SendAsync(request);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);

            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("Username must be between 5 and 20 characters");

            //Clean up
            context.User.Remove(testUser);
            await context.SaveChangesAsync();
        }

        [Fact]
        public async Task UpdateUserProfile_WithNameTooShort_ShouldReturnBadRequest()
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
            var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

            var testUser = await _helperService.CreateTestUserAsync();
            var token = tokenService.GenerateToken(testUser.Id, ((RoleEnum)testUser.RoleId).ToString());

            var updateRequest = new UpdateUserProfileRequest
            {
                Username = "updatedusername",
                Email = _helperService.CreateRandomEmail(),
                Name = "", //too short
                Surname = _helperService.CreateRandomText()
            };

            var request = new HttpRequestMessage(HttpMethod.Put, "/User/UpdateUserProfile")
            {
                Content = JsonContent.Create(updateRequest)
            };
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _client.SendAsync(request);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);

            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("Name must be between 1 and 100 characters");

            //Clean up
            context.User.Remove(testUser);
            await context.SaveChangesAsync();
        }

        [Fact]
        public async Task UpdateUserProfile_WithNameTooLong_ShouldReturnBadRequest()
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
            var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

            var testUser = await _helperService.CreateTestUserAsync();
            var token = tokenService.GenerateToken(testUser.Id, ((RoleEnum)testUser.RoleId).ToString());

            var updateRequest = new UpdateUserProfileRequest
            {
                Username = "updatedusername",
                Email = _helperService.CreateRandomEmail(),
                Name = new string('A', 101), //too long (max length = 100)
                Surname = _helperService.CreateRandomText()
            };

            var request = new HttpRequestMessage(HttpMethod.Put, "/User/UpdateUserProfile")
            {
                Content = JsonContent.Create(updateRequest)
            };
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _client.SendAsync(request);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);

            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("Name must be between 1 and 100 characters");

            //Clean up
            context.User.Remove(testUser);
            await context.SaveChangesAsync();
        }

        [Fact]
        public async Task UpdateUserProfile_WithSurnameTooShort_ShouldReturnBadRequest()
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
            var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

            var testUser = await _helperService.CreateTestUserAsync();
            var token = tokenService.GenerateToken(testUser.Id, ((RoleEnum)testUser.RoleId).ToString());

            var updateRequest = new UpdateUserProfileRequest
            {
                Username = "updatedusername",
                Email = _helperService.CreateRandomEmail(),
                Name = _helperService.CreateRandomText(),
                Surname = "" //too short
            };

            var request = new HttpRequestMessage(HttpMethod.Put, "/User/UpdateUserProfile")
            {
                Content = JsonContent.Create(updateRequest)
            };
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _client.SendAsync(request);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);

            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("Surname must be between 1 and 100 characters");

            //Clean up
            context.User.Remove(testUser);
            await context.SaveChangesAsync();
        }

        [Fact]
        public async Task UpdateUserProfile_WithSurnameTooLong_ShouldReturnBadRequest()
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
            var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

            var testUser = await _helperService.CreateTestUserAsync();
            var token = tokenService.GenerateToken(testUser.Id, ((RoleEnum)testUser.RoleId).ToString());

            var updateRequest = new UpdateUserProfileRequest
            {
                Username = "updatedusername",
                Email = _helperService.CreateRandomEmail(),
                Name = _helperService.CreateRandomText(),
                Surname = new string('B', 101) //too long (max length = 100)
            };

            var request = new HttpRequestMessage(HttpMethod.Put, "/User/UpdateUserProfile")
            {
                Content = JsonContent.Create(updateRequest)
            };
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _client.SendAsync(request);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);

            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("Surname must be between 1 and 100 characters");

            //Clean up
            context.User.Remove(testUser);
            await context.SaveChangesAsync();
        }
    }
}