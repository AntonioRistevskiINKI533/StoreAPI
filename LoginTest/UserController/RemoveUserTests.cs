using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using StoreAPI.Services;
using System.Net.Http;
using StoreAPI.Models.Contexts;
using StoreAPI.IntegrationTests.Shared;

public class RemoveUserIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;
    private readonly HelperService _helperService;

    public RemoveUserIntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
        _helperService = new HelperService(_factory);
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

        var userToRemove = await _helperService.CreateTestUserAsync();

        context.User.Remove(userToRemove);
        await context.SaveChangesAsync();

        var adminUser = await _helperService.CreateTestUserAsync(true);

        var adminToken = tokenService.GenerateToken(adminUser.Id, "Admin");

        // prepare request
        var request = new HttpRequestMessage(HttpMethod.Delete, $"/User/RemoveUser?userId={userToRemove.Id}");
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", adminToken);

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);

        context.User.Remove(adminUser);
        await context.SaveChangesAsync();
    }
}
