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

public class GetUserProfileIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;
    private readonly HelperService _helperService;

    public GetUserProfileIntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
        _helperService = new HelperService(_factory);
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
        profile!.Username.Should().Be(testUser.Username);
        profile.Email.Should().Be(testUser.Email);

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
    public async Task GetUserProfile_NonExistentUser_ShouldReturnUnauthorized()
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
}
