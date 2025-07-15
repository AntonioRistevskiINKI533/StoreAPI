using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using StoreAPI.Services;
using System.Net.Http;
using StoreAPI.Models.Contexts;
using StoreAPI.IntegrationTests.Shared;
using Microsoft.EntityFrameworkCore;
using StoreAPI.Enums;
using StoreAPI.Models.Requests;
using System.Net.Http.Json;

public class AddCompanyIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;
    private readonly HelperService _helperService;

    public AddCompanyIntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
        _helperService = new HelperService(_factory);
    }

    [Fact]
    public async Task AddCompany_WithValidData_ShouldReturnOk()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
        var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

        var testUser = await _helperService.CreateTestUserAsync();
        var token = tokenService.GenerateToken(testUser.Id, ((RoleEnum)testUser.RoleId).ToString());

        var addRequest = new AddCompanyRequest
        {
            Name = "newcompany",
            Address = "newcompanyaddress",
            Phone = "+389077123123"
        };

        var request = new HttpRequestMessage(HttpMethod.Post, "/Company/AddCompany")
        {
            Content = JsonContent.Create(addRequest)
        };
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

        var addedCompany = await context.Company.FirstOrDefaultAsync(u => u.Name == addRequest.Name);
        addedCompany.Should().NotBeNull();
        addedCompany.Name.Should().Be(addRequest.Name);
        addedCompany.Address.Should().Be(addRequest.Address);
        addedCompany.Phone.Should().Be(addRequest.Phone);

        //Clean up
        context.Company.Remove(addedCompany);
        context.User.Remove(testUser);
        await context.SaveChangesAsync();
    }

    [Fact]
    public async Task AddCompany_WithExistingName_ShouldReturnBadRequest()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
        var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

        var testUser = await _helperService.CreateTestUserAsync();
        var token = tokenService.GenerateToken(testUser.Id, ((RoleEnum)testUser.RoleId).ToString());

        var testCompany = await _helperService.CreateTestCompanyAsync();

        var addRequest = new AddCompanyRequest
        {
            Name = testCompany.Name,
            Address = "newcompanyaddress",
            Phone = "+389077123123"
        };

        var request = new HttpRequestMessage(HttpMethod.Post, "/Company/AddCompany")
        {
            Content = JsonContent.Create(addRequest)
        };
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);

        //Clean up
        context.Company.Remove(testCompany);
        context.User.Remove(testUser);
        await context.SaveChangesAsync();
    }

    [Fact]
    public async Task AddCompany_WithExistingAddress_ShouldReturnBadRequest()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
        var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

        var testUser = await _helperService.CreateTestUserAsync();
        var token = tokenService.GenerateToken(testUser.Id, ((RoleEnum)testUser.RoleId).ToString());

        var testCompany = await _helperService.CreateTestCompanyAsync();

        var addRequest = new AddCompanyRequest
        {
            Name = "newcompany",
            Address = testCompany.Address,
            Phone = "+389077123123"
        };

        var request = new HttpRequestMessage(HttpMethod.Post, "/Company/AddCompany")
        {
            Content = JsonContent.Create(addRequest)
        };
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);

        //Clean up
        context.Company.Remove(testCompany);
        context.User.Remove(testUser);
        await context.SaveChangesAsync();
    }

    [Fact]
    public async Task AddCompany_WithExistingPhone_ShouldReturnBadRequest()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
        var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

        var testUser = await _helperService.CreateTestUserAsync();
        var token = tokenService.GenerateToken(testUser.Id, ((RoleEnum)testUser.RoleId).ToString());

        var testCompany = await _helperService.CreateTestCompanyAsync();

        var addRequest = new AddCompanyRequest
        {
            Name = "newcompany",
            Address = "newcompanyaddress",
            Phone = testCompany.Phone
        };

        var request = new HttpRequestMessage(HttpMethod.Post, "/Company/AddCompany")
        {
            Content = JsonContent.Create(addRequest)
        };
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);

        //Clean up
        context.Company.Remove(testCompany);
        context.User.Remove(testUser);
        await context.SaveChangesAsync();
    }

    [Fact]
    public async Task AddUser_WithInvalidPhoneFormat_ShouldReturnBadRequest()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
        var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

        var testUser = await _helperService.CreateTestUserAsync();
        var token = tokenService.GenerateToken(testUser.Id, ((RoleEnum)testUser.RoleId).ToString());

        var addRequest = new AddCompanyRequest
        {
            Name = "newcompany",
            Address = "newcompanyaddress",
            Phone = "abc"
        };

        var request = new HttpRequestMessage(HttpMethod.Post, "/Company/AddCompany")
        {
            Content = JsonContent.Create(addRequest)
        };
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);

        //Clean up
        context.User.Remove(testUser);
        await context.SaveChangesAsync();
    }

    [Fact]
    public async Task AddUser_WithNameTooShort_ShouldReturnBadRequest()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
        var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

        var testUser = await _helperService.CreateTestUserAsync();
        var token = tokenService.GenerateToken(testUser.Id, ((RoleEnum)testUser.RoleId).ToString());

        var addRequest = new AddCompanyRequest
        {
            Name = "",
            Address = "newcompanyaddress",
            Phone = "+389077123123" //invalid phone
        };

        var request = new HttpRequestMessage(HttpMethod.Post, "/Company/AddCompany")
        {
            Content = JsonContent.Create(addRequest)
        };
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);

        //Clean up
        context.User.Remove(testUser);
        await context.SaveChangesAsync();
    }

    [Fact]
    public async Task AddUser_WithNameTooLong_ShouldReturnBadRequest()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
        var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

        var testUser = await _helperService.CreateTestUserAsync();
        var token = tokenService.GenerateToken(testUser.Id, ((RoleEnum)testUser.RoleId).ToString());

        var addRequest = new AddCompanyRequest
        {
            Name = new string('A', 501),
            Address = "newcompanyaddress",
            Phone = "+389077123123"
        };

        var request = new HttpRequestMessage(HttpMethod.Post, "/Company/AddCompany")
        {
            Content = JsonContent.Create(addRequest)
        };
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);

        //Clean up
        context.User.Remove(testUser);
        await context.SaveChangesAsync();
    }

    [Fact]
    public async Task AddUser_WithAddressTooShort_ShouldReturnBadRequest()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
        var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

        var testUser = await _helperService.CreateTestUserAsync();
        var token = tokenService.GenerateToken(testUser.Id, ((RoleEnum)testUser.RoleId).ToString());

        var addRequest = new AddCompanyRequest
        {
            Name = "newcompany",
            Address = "abcd",
            Phone = "+389077123123"
        };

        var request = new HttpRequestMessage(HttpMethod.Post, "/Company/AddCompany")
        {
            Content = JsonContent.Create(addRequest)
        };
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);

        //Clean up
        context.User.Remove(testUser);
        await context.SaveChangesAsync();
    }

    [Fact]
    public async Task AddUser_WithAddressTooLong_ShouldReturnBadRequest()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
        var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

        var testUser = await _helperService.CreateTestUserAsync();
        var token = tokenService.GenerateToken(testUser.Id, ((RoleEnum)testUser.RoleId).ToString());

        var addRequest = new AddCompanyRequest
        {
            Name = "newcompany",
            Address = new string('A', 21),
            Phone = "+389077123123"
        };

        var request = new HttpRequestMessage(HttpMethod.Post, "/Company/AddCompany")
        {
            Content = JsonContent.Create(addRequest)
        };
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);

        //Clean up
        context.User.Remove(testUser);
        await context.SaveChangesAsync();
    }
}
