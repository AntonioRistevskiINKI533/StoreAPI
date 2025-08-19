using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using StoreAPI.Models.Contexts;
using StoreAPI.Models.Requests;
using StoreAPI.IntegrationTests.Shared;
using StoreAPI.Models.Responses;
using System.Linq;
using System;

namespace StoreAPI.IntegrationTests.UserController
{
    public class LoginIntegrationTests : IClassFixture<CustomWebApplicationFactory>, IDisposable
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory _factory;
        private readonly HelperService _helperService;
        private readonly string prefix = "Login_";

        public LoginIntegrationTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
            _helperService = new HelperService(_factory);
        }

        [Fact]
        public async Task Login_WithValidCredentials_ShouldReturnToken()
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<StoreContext>();

            var testUser = await _helperService.CreateTestUserAsync(prefix);

            var loginRequest = new LoginRequest
            {
                Username = testUser.Username,
                Password = "Pa$$w0rd!"
            };

            var response = await _client.PostAsJsonAsync("/User/Login", loginRequest);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

            var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();
            loginResponse.Should().NotBeNull();
            loginResponse.Token.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task Login_WithInvalidCredentials_ShouldReturnUnauthorized()
        {
            var loginRequest = new LoginRequest
            {
                Username = "nonexistent",
                Password = "wrongpassword"
            };

            var response = await _client.PostAsJsonAsync("/User/Login", loginRequest);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
        }

        public void Dispose()
        {
            _helperService.CleanUp(prefix);
        }
    }
}