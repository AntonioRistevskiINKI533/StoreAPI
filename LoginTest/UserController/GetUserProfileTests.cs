using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using StoreAPI.Services;
using System.Net.Http;
using StoreAPI.Models.Contexts;
using StoreAPI.Models.Datas;
using StoreAPI.IntegrationTests.Shared;
using StoreAPI.Enums;
using System.Linq;
using System;

namespace StoreAPI.IntegrationTests.UserController
{
    public class GetUserProfileIntegrationTests : IClassFixture<CustomWebApplicationFactory>, IDisposable
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory _factory;
        private readonly HelperService _helperService;
        private readonly string prefix = "GetUserProfile_";

        public GetUserProfileIntegrationTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
            _helperService = new HelperService(_factory);
        }

        [Fact]
        public async Task GetUserProfile_WithValidToken_ShouldReturnUserProfile()
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
            var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

            var testUser = await _helperService.CreateTestUserAsync(prefix);

            var token = tokenService.GenerateToken(testUser.Id, ((RoleEnum)testUser.RoleId).ToString());

            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _client.GetAsync("/User/GetUserProfile");

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

            var profile = await response.Content.ReadFromJsonAsync<UserData>();
            profile.Should().NotBeNull();
            profile!.Username.Should().Be(testUser.Username);
            profile.Email.Should().Be(testUser.Email);
        }

        [Fact]
        public async Task GetUserProfile_WithoutToken_ShouldReturnUnauthorized()
        {
            var response = await _client.GetAsync("/User/GetUserProfile");

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task GetUserProfile_NonExistentUser_ShouldReturnUnauthorized()
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
            var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

            var testUser = await _helperService.CreateTestUserAsync(prefix);

            var token = tokenService.GenerateToken(testUser.Id, ((RoleEnum)testUser.RoleId).ToString());

            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            //User is removed to simulate non-existent user
            context.User.Remove(testUser);
            await context.SaveChangesAsync();

            var response = await _client.GetAsync("/User/GetUserProfile");

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);

            var content = await response.Content.ReadAsStringAsync();
            content.Should().Be("User does not exist");
        }

        public void Dispose()
        {
            _helperService.CleanUp(prefix);
        }
    }
}