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
        // Arrange: create test user and generate token
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
        var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

        var testUser = await _helperService.CreateTestUserAsync();

        var token = tokenService.GenerateToken(testUser.Id, "Employee");

        var updateRequest = new UpdateUserProfileRequest
        {
            Username = "updatedusername",
            Email = "updatedemail@example.com",
            Name = "UpdatedName",
            Surname = "UpdatedSurname"
        };

        var request = new HttpRequestMessage(HttpMethod.Put, "/User/UpdateUserProfile")
        {
            Content = JsonContent.Create(updateRequest)
        };
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

        // Optionally verify user data is updated in DB
        var updatedUser = await context.User.FindAsync(testUser.Id);
        updatedUser.Should().NotBeNull();
        updatedUser!.Name.Should().Be("UpdatedName");
        updatedUser.Surname.Should().Be("UpdatedSurname");
        updatedUser.Email.Should().Be("updatedemail@example.com");

        // Clean up
        context.User.Remove(updatedUser);
        await context.SaveChangesAsync();
    }

}
