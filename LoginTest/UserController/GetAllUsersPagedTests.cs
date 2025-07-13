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
using StoreAPI.Models.Datas;
using System.Net;
using StoreAPI.Enums;

public class GetAllUsersPagedIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;
    private readonly HelperService _helperService;

    public GetAllUsersPagedIntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
        _helperService = new HelperService(_factory);
    }

    [Fact]
    public async Task GetAllUsersPaged_WithValidToken_ShouldReturnOkAndPagedResult()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
        var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

        var testUser = await _helperService.CreateTestUserAsync();
        var token = tokenService.GenerateToken(testUser.Id, ((RoleEnum)testUser.RoleId).ToString());

        var anotherUser1 = await _helperService.CreateTestUserAsync();
        var anotherUser2 = await _helperService.CreateTestUserAsync();

        var request = new HttpRequestMessage(HttpMethod.Get, "/User/GetAllUsersPaged?pageIndex=0&pageSize=10");
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<PagedModel<UserData>>();
        result.Should().NotBeNull();
        result.Items.Should().NotBeNull();
        result.Items.Count.Should().BeGreaterThanOrEqualTo(3); //should contain at least the three created users
        //TODO create TEST  database

        //Clean up
        context.User.Remove(testUser);
        context.User.Remove(anotherUser1);
        context.User.Remove(anotherUser2);
        await context.SaveChangesAsync();
    }

    [Fact]
    public async Task GetAllUsersPaged_WithFullNameFilter_ShouldReturnFilteredResults()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
        var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

        var testUser = await _helperService.CreateTestUserAsync();
        await context.SaveChangesAsync();

        var token = tokenService.GenerateToken(testUser.Id, ((RoleEnum)testUser.RoleId).ToString());

        var request = new HttpRequestMessage(HttpMethod.Get, $"/User/GetAllUsersPaged?pageIndex=0&pageSize=10&fullName={testUser.Name}");
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<PagedModel<UserData>>();
        result.Should().NotBeNull();
        result!.Items.Should().Contain(u => u.Name == testUser.Name);

        //Clean up
        context.User.Remove(testUser);
        await context.SaveChangesAsync();
    }

    [Fact]
    public async Task GetAllUsersPaged_WithRoleIdFilter_ShouldReturnFilteredResults()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
        var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

        var adminUser = await _helperService.CreateTestUserAsync(true);
        var token = tokenService.GenerateToken(adminUser.Id, ((RoleEnum)adminUser.RoleId).ToString());

        var employeeUser = await _helperService.CreateTestUserAsync(isAdmin: false);

        var request = new HttpRequestMessage(HttpMethod.Get, $"/User/GetAllUsersPaged?pageIndex=0&pageSize=10&roleId={(int)RoleEnum.Employee}");
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<PagedModel<UserData>>();
        result.Should().NotBeNull();
        result!.Items.Should().Contain(u => u.Id == employeeUser.Id);

        //Clean up
        context.User.Remove(adminUser);
        context.User.Remove(employeeUser);
        await context.SaveChangesAsync();
    }
}
