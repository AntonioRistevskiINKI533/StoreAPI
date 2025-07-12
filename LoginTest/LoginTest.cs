using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using System;
using Microsoft.Extensions.DependencyInjection;
using StoreAPI.Models;
using StoreAPI.Services;
using System.Net.Http;
using StoreAPI.Models.Contexts;
using StoreAPI.Models.Requests;
using StoreAPI.Enums;
using StoreAPI.Models.Datas;
using StoreAPI.IntegrationTests.Shared;

public class UserIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;
    private readonly HelperService _helperService;

    public UserIntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
        _helperService = new HelperService(_factory);
    }

    [Fact]
    public async Task Login_WithValidCredentials_ShouldReturnToken()
    {
        // Arrange: create test user in database
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<StoreContext>();

        var testUser = await _helperService.CreateTestUserAsync();

        var loginRequest = new LoginRequest
        {
            Username = "testuser",
            Password = "Pa$$w0rd!"
        };

        // Act: send login request
        var response = await _client.PostAsJsonAsync("/User/Login", loginRequest);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

        var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();
        loginResponse.Should().NotBeNull();
        loginResponse!.Token.Should().NotBeNullOrEmpty();

        context.User.Remove(testUser);
        await context.SaveChangesAsync();
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

    [Fact]
    public async Task GetUserProfile_WithValidToken_ShouldReturnUserProfile()
    {
        // Arrange: create test user and generate token
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
        var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

        var testUser = await _helperService.CreateTestUserAsync();

        // generate JWT token
        var token = tokenService.GenerateToken(testUser.Id, "Employee");

        // attach token to request
        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/User/GetUserProfile");

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

        var profile = await response.Content.ReadFromJsonAsync<UserData>();
        profile.Should().NotBeNull();
        profile!.Username.Should().Be("testuser");
        profile.Email.Should().Be("testuser@example.com");

        // Cleanup
        context.User.Remove(testUser);
        await context.SaveChangesAsync();
    }

    [Fact]
    public async Task GetUserProfile_WithoutToken_ShouldReturnUnauthorized()
    {
        // Act
        var response = await _client.GetAsync("/User/GetUserProfile");

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetUserProfile_NotExistentUser_ShouldReturnUnauthorized()
    {
        // Arrange: create test user and generate token
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
        var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

        var testUser = await _helperService.CreateTestUserAsync();

        // generate JWT token
        var token = tokenService.GenerateToken(testUser.Id, "Employee");

        // attach token to request
        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // User is removed to simulate non-existent user
        context.User.Remove(testUser);
        await context.SaveChangesAsync();

        // Act
        var response = await _client.GetAsync("/User/GetUserProfile");

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task RemoveUser_WithValidUserIdAndAdminToken_ShouldReturnOk()
    {
        // Arrange: create user to be deleted
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
        var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

        var userToRemove = await _helperService.CreateTestUserAsync();

        var adminUser = await _helperService.CreateTestUserAsync(true);

        // generate token for admin
        var adminToken = tokenService.GenerateToken(adminUser.Id, "Admin");

        // prepare request
        var request = new HttpRequestMessage(HttpMethod.Delete, $"/User/RemoveUser?userId={userToRemove.Id}");
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", adminToken);

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

        context.User.Remove(adminUser);
        await context.SaveChangesAsync();
    }

    [Fact]
    public async Task RemoveUser_WithNonExistentUserId_ShouldReturnNotFound()
    {
        // Arrange: create admin user and token
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
        var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

        var adminUser = await _helperService.CreateTestUserAsync(true);

        var adminToken = tokenService.GenerateToken(adminUser.Id, "Admin");

        // prepare request with non-existent userId
        int nonExistentUserId = 999999;
        var request = new HttpRequestMessage(HttpMethod.Delete, $"/User/RemoveUser?userId={nonExistentUserId}");
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", adminToken);

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);

        context.User.Remove(adminUser);
        await context.SaveChangesAsync();
    }
}
