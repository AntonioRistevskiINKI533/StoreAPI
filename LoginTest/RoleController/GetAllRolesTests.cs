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
using System.Collections.Generic;

public class GetAllRolesIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;
    private readonly HelperService _helperService;

    public GetAllRolesIntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
        _helperService = new HelperService(_factory);
    }

    [Fact]
    public async Task GetAllRoles_WithValidToken_ShouldReturnListOfRoles()
    {
        // Arrange: create test user and generate token
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
        var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

        var testUser = await _helperService.CreateTestUserAsync();
        var token = tokenService.GenerateToken(testUser.Id, "Employee");

        // prepare request
        var request = new HttpRequestMessage(HttpMethod.Get, "/Role/GetAllRoles");
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

        var roles = await response.Content.ReadFromJsonAsync<List<RoleData>>();
        roles.Should().NotBeNull();
        roles.Should().NotBeEmpty();

        // Clean up
        context.User.Remove(testUser);
        await context.SaveChangesAsync();
    }

}
