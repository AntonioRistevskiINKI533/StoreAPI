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

public class GetUserIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;
    private readonly HelperService _helperService;

    public GetUserIntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
        _helperService = new HelperService(_factory);
    }

    [Fact]
    public async Task GetUser_WithValidUserIdAndToken_ShouldReturnOk()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
        var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

        var adminUser = await _helperService.CreateTestUserAsync(isAdmin: true);

        var testUser = await _helperService.CreateTestUserAsync();

        var token = tokenService.GenerateToken(adminUser.Id, "Employee");

        var request = new HttpRequestMessage(HttpMethod.Get, $"/User/GetUser?userId={testUser.Id}");
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

        var userData = await response.Content.ReadFromJsonAsync<UserData>();
        userData.Should().NotBeNull();
        userData!.Id.Should().Be(testUser.Id);
        userData.Username.Should().Be(testUser.Username);
        userData.Email.Should().Be(testUser.Email);
        userData.Name.Should().Be(testUser.Name);
        userData.Surname.Should().Be(testUser.Surname);
        userData.RoleId.Should().Be(testUser.RoleId);

        //Cleanup
        context.User.Remove(adminUser);
        context.User.Remove(testUser);
        await context.SaveChangesAsync();
    }

    [Fact]
    public async Task GetUser_WithNonExistentUserId_ShouldReturnNotFound()
    {
        using var scope = _factory.Services.CreateScope();
        using var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
        var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

        var adminUser = await _helperService.CreateTestUserAsync(isAdmin: true);

        var testUser = await _helperService.CreateTestUserAsync();

        var token = tokenService.GenerateToken(testUser.Id, "Employee");

        context.User.Remove(testUser);
        await context.SaveChangesAsync();

        var request = new HttpRequestMessage(HttpMethod.Get, $"/User/GetUser?userId={testUser.Id}");
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);

        //Cleanup
        context.User.Remove(adminUser);
        await context.SaveChangesAsync();
    }
}
