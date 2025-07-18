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

public class UpdateCompanyIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;
    private readonly HelperService _helperService;

    public UpdateCompanyIntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
        _helperService = new HelperService(_factory);
    }

    [Fact]
    public async Task UpdateCompany_WithValidData_ShouldReturnOk()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
        var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

        var testUser = await _helperService.CreateTestUserAsync();
        var token = tokenService.GenerateToken(testUser.Id, ((RoleEnum)testUser.RoleId).ToString());

        var testCompany = await _helperService.CreateTestCompanyAsync();

        var updateRequest = new UpdateCompanyRequest
        {
            Id = testCompany.Id,
            Name = "updatedcompany",
            Address = "updatedcompanyaddress",
            Phone = "+389077123123"
        };

        var request = new HttpRequestMessage(HttpMethod.Put, "/Company/UpdateCompany")
        {
            Content = JsonContent.Create(updateRequest)
        };
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

        var updatedCompany = await context.Company.FirstOrDefaultAsync(u => u.Id == updateRequest.Id);
        updatedCompany.Should().NotBeNull();
        updatedCompany.Name.Should().Be(updateRequest.Name);
        updatedCompany.Address.Should().Be(updateRequest.Address);
        updatedCompany.Phone.Should().Be(updateRequest.Phone);

        //Clean up
        context.Company.Remove(updatedCompany);
        context.User.Remove(testUser);
        await context.SaveChangesAsync();
    }

    [Fact]
    public async Task UpdateCompany_WithExistingName_ShouldReturnBadRequest()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
        var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

        var testUser = await _helperService.CreateTestUserAsync();
        var token = tokenService.GenerateToken(testUser.Id, ((RoleEnum)testUser.RoleId).ToString());

        var testCompany = await _helperService.CreateTestCompanyAsync();

        var existingCompany = await _helperService.CreateTestCompanyAsync();

        var updateRequest = new UpdateCompanyRequest
        {
            Id = testCompany.Id,
            Name = existingCompany.Name,
            Address = "updatedcompanyaddress",
            Phone = "+389077123123"
        };

        var request = new HttpRequestMessage(HttpMethod.Put, "/Company/UpdateCompany")
        {
            Content = JsonContent.Create(updateRequest)
        };
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        //TODO maybe it hould relly return Conflict instead of badRequest

        //Clean up
        context.Company.Remove(testCompany);
        context.Company.Remove(existingCompany);
        context.User.Remove(testUser);
        await context.SaveChangesAsync();
    }

    [Fact]
    public async Task UpdateCompany_WithExistingAddress_ShouldReturnBadRequest()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
        var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

        var testUser = await _helperService.CreateTestUserAsync();
        var token = tokenService.GenerateToken(testUser.Id, ((RoleEnum)testUser.RoleId).ToString());

        var testCompany = await _helperService.CreateTestCompanyAsync();

        var existingCompany = await _helperService.CreateTestCompanyAsync();

        var updateRequest = new UpdateCompanyRequest
        {
            Id = testCompany.Id,
            Name = "updatedcompany",
            Address = existingCompany.Address,
            Phone = "+389077123123"
        };

        var request = new HttpRequestMessage(HttpMethod.Put, "/Company/UpdateCompany")
        {
            Content = JsonContent.Create(updateRequest)
        };
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        //TODO maybe it hould relly return Conflict instead of badRequest

        //Clean up
        context.Company.Remove(testCompany);
        context.Company.Remove(existingCompany);
        context.User.Remove(testUser);
        await context.SaveChangesAsync();
    }

    [Fact]
    public async Task UpdateCompany_WithExistingPhone_ShouldReturnBadRequest()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
        var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

        var testUser = await _helperService.CreateTestUserAsync();
        var token = tokenService.GenerateToken(testUser.Id, ((RoleEnum)testUser.RoleId).ToString());

        var testCompany = await _helperService.CreateTestCompanyAsync();

        var existingCompany = await _helperService.CreateTestCompanyAsync();

        var updateRequest = new UpdateCompanyRequest
        {
            Id = testCompany.Id,
            Name = "updatedcompany",
            Address = "updatedcompanyaddress",
            Phone = existingCompany.Phone
        };

        var request = new HttpRequestMessage(HttpMethod.Put, "/Company/UpdateCompany")
        {
            Content = JsonContent.Create(updateRequest)
        };
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        //TODO maybe it hould relly return Conflict instead of badRequest

        //Clean up
        context.Company.Remove(testCompany);
        context.Company.Remove(existingCompany);
        context.User.Remove(testUser);
        await context.SaveChangesAsync();
    }

    [Fact]
    public async Task UpdateUser_WithInvalidPhoneFormat_ShouldReturnBadRequest()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
        var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

        var testUser = await _helperService.CreateTestUserAsync();
        var token = tokenService.GenerateToken(testUser.Id, ((RoleEnum)testUser.RoleId).ToString());

        var testCompany = await _helperService.CreateTestCompanyAsync();

        var updateRequest = new UpdateCompanyRequest
        {
            Id = testCompany.Id,
            Name = "updatedcompany",
            Address = "updatedcompanyaddress",
            Phone = "abc"
        };

        var request = new HttpRequestMessage(HttpMethod.Put, "/Company/UpdateCompany")
        {
            Content = JsonContent.Create(updateRequest)
        };
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        //TODO maybe it hould relly return Conflict instead of badRequest

        //Clean up
        context.Company.Remove(testCompany);
        context.User.Remove(testUser);
        await context.SaveChangesAsync();
    }

    [Fact]
    public async Task UpdateUser_WithNameTooShort_ShouldReturnBadRequest()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
        var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

        var testUser = await _helperService.CreateTestUserAsync();
        var token = tokenService.GenerateToken(testUser.Id, ((RoleEnum)testUser.RoleId).ToString());

        var testCompany = await _helperService.CreateTestCompanyAsync();

        var updateRequest = new UpdateCompanyRequest
        {
            Id = testCompany.Id,
            Name = "",
            Address = "updatedcompanyaddress",
            Phone = "+389077123123"
        };

        var request = new HttpRequestMessage(HttpMethod.Put, "/Company/UpdateCompany")
        {
            Content = JsonContent.Create(updateRequest)
        };
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        //TODO maybe it hould relly return Conflict instead of badRequest

        //Clean up
        context.Company.Remove(testCompany);
        context.User.Remove(testUser);
        await context.SaveChangesAsync();
    }

    [Fact]
    public async Task UpdateUser_WithNameTooLong_ShouldReturnBadRequest()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
        var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

        var testUser = await _helperService.CreateTestUserAsync();
        var token = tokenService.GenerateToken(testUser.Id, ((RoleEnum)testUser.RoleId).ToString());

        var testCompany = await _helperService.CreateTestCompanyAsync();

        var updateRequest = new UpdateCompanyRequest
        {
            Id = testCompany.Id,
            Name = new string('A', 501),
            Address = "updatedcompanyaddress",
            Phone = "+389077123123"
        };

        var request = new HttpRequestMessage(HttpMethod.Put, "/Company/UpdateCompany")
        {
            Content = JsonContent.Create(updateRequest)
        };
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        //TODO maybe it hould relly return Conflict instead of badRequest

        //Clean up
        context.Company.Remove(testCompany);
        context.User.Remove(testUser);
        await context.SaveChangesAsync();
    }

    [Fact]
    public async Task UpdateUser_WithAddressTooShort_ShouldReturnBadRequest()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
        var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

        var testUser = await _helperService.CreateTestUserAsync();
        var token = tokenService.GenerateToken(testUser.Id, ((RoleEnum)testUser.RoleId).ToString());

        var testCompany = await _helperService.CreateTestCompanyAsync();

        var updateRequest = new UpdateCompanyRequest
        {
            Id = testCompany.Id,
            Name = "updatedcompany",
            Address = "",
            Phone = "+389077123123"
        };

        var request = new HttpRequestMessage(HttpMethod.Put, "/Company/UpdateCompany")
        {
            Content = JsonContent.Create(updateRequest)
        };
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        //TODO maybe it hould relly return Conflict instead of badRequest

        //Clean up
        context.Company.Remove(testCompany);
        context.User.Remove(testUser);
        await context.SaveChangesAsync();
    }

    [Fact]
    public async Task UpdateUser_WithAddressTooLong_ShouldReturnBadRequest()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
        var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

        var testUser = await _helperService.CreateTestUserAsync();
        var token = tokenService.GenerateToken(testUser.Id, ((RoleEnum)testUser.RoleId).ToString());

        var testCompany = await _helperService.CreateTestCompanyAsync();

        var updateRequest = new UpdateCompanyRequest
        {
            Id = testCompany.Id,
            Name = "updatedcompany",
            Address = new string('A', 21),
            Phone = "+389077123123"
        };

        var request = new HttpRequestMessage(HttpMethod.Put, "/Company/UpdateCompany")
        {
            Content = JsonContent.Create(updateRequest)
        };
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        //TODO maybe it hould relly return Conflict instead of badRequest

        //Clean up
        context.Company.Remove(testCompany);
        context.User.Remove(testUser);
        await context.SaveChangesAsync();
    }
}
