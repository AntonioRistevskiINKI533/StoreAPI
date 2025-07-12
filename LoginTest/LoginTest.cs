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

public class UserIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public UserIntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Login_WithValidCredentials_ShouldReturnToken()
    {
        // Arrange: create test user in database
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
}
