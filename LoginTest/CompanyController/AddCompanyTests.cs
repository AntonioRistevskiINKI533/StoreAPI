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

    /*[Fact]
    public async Task AddUser_WithExistingEmail_ShouldReturnBadRequest()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
        var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

        var adminUser = await _helperService.CreateTestUserAsync(true);
        var existingUser = await _helperService.CreateTestUserAsync();

        var token = tokenService.GenerateToken(adminUser.Id, ((RoleEnum)adminUser.RoleId).ToString());

        var addRequest = new AddUserRequest
        {
            Username = "newuser",
            Email = existingUser.Email, //duplicate email
            Name = "NewName",
            Surname = "NewSurname",
            RoleId = (int)RoleEnum.Employee,
            Password = "Pa$$w0rd!"
        };

        var request = new HttpRequestMessage(HttpMethod.Post, "/User/AddUser")
        {
            Content = JsonContent.Create(addRequest)
        };
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);

        //Clean up
        context.User.Remove(adminUser);
        context.User.Remove(existingUser);
        await context.SaveChangesAsync();
    }

    [Fact]
    public async Task AddUser_WithInvalidEmailFormat_ShouldReturnBadRequest()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
        var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

        var adminUser = await _helperService.CreateTestUserAsync(true);
        var token = tokenService.GenerateToken(adminUser.Id, ((RoleEnum)adminUser.RoleId).ToString());

        var addRequest = new AddUserRequest
        {
            Username = "newuser",
            Email = "invalid-email", //invalid
            Name = "NewName",
            Surname = "NewSurname",
            RoleId = (int)RoleEnum.Employee,
            Password = "Pa$$w0rd!"
        };

        var request = new HttpRequestMessage(HttpMethod.Post, "/User/AddUser")
        {
            Content = JsonContent.Create(addRequest)
        };
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);

        //Clean up
        context.User.Remove(adminUser);
        await context.SaveChangesAsync();
    }

    [Fact]
    public async Task AddUser_WithPasswordTooShort_ShouldReturnBadRequest()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
        var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

        var adminUser = await _helperService.CreateTestUserAsync(true);
        var token = tokenService.GenerateToken(adminUser.Id, ((RoleEnum)adminUser.RoleId).ToString());

        var addRequest = new AddUserRequest
        {
            Username = "newuser",
            Email = "newuser@example.com",
            Name = "NewName",
            Surname = "NewSurname",
            RoleId = (int)RoleEnum.Employee,
            Password = "short" //too short
        };

        var request = new HttpRequestMessage(HttpMethod.Post, "/User/AddUser")
        {
            Content = JsonContent.Create(addRequest)
        };
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);

        //Clean up
        context.User.Remove(adminUser);
        await context.SaveChangesAsync();
    }

    [Fact]
    public async Task AddUser_WithNonExistentRoleId_ShouldReturnBadRequest()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
        var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

        var adminUser = await _helperService.CreateTestUserAsync(true);
        var token = tokenService.GenerateToken(adminUser.Id, ((RoleEnum)adminUser.RoleId).ToString());

        var addRequest = new AddUserRequest
        {
            Username = "newuser",
            Email = "newuser@example.com",
            Name = "NewName",
            Surname = "NewSurname",
            RoleId = 9999, //non-existent role
            Password = "Pa$$w0rd!"
        };

        var request = new HttpRequestMessage(HttpMethod.Post, "/User/AddUser")
        {
            Content = JsonContent.Create(addRequest)
        };
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);

        //Clean up
        context.User.Remove(adminUser);
        await context.SaveChangesAsync();
    }

    [Fact]
    public async Task AddUser_WithUsernameTooShort_ShouldReturnBadRequest()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
        var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

        var adminUser = await _helperService.CreateTestUserAsync(true);
        var token = tokenService.GenerateToken(adminUser.Id, ((RoleEnum)adminUser.RoleId).ToString());

        var addRequest = new AddUserRequest
        {
            Username = "abc", //too short (min length = 5)
            Email = "newuser@example.com",
            Name = "NewName",
            Surname = "NewSurname",
            RoleId = (int)RoleEnum.Employee,
            Password = "Pa$$w0rd!"
        };

        var request = new HttpRequestMessage(HttpMethod.Post, "/User/AddUser")
        {
            Content = JsonContent.Create(addRequest)
        };
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);

        //Clean up
        context.User.Remove(adminUser);
        await context.SaveChangesAsync();
    }

    [Fact]
    public async Task AddUser_WithUsernameTooLong_ShouldReturnBadRequest()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
        var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

        var adminUser = await _helperService.CreateTestUserAsync(true);
        var token = tokenService.GenerateToken(adminUser.Id, ((RoleEnum)adminUser.RoleId).ToString());

        var addRequest = new AddUserRequest
        {
            Username = new string('A', 21), //too long
            Email = "newuser@example.com",
            Name = "NewName",
            Surname = "NewSurname",
            RoleId = (int)RoleEnum.Employee,
            Password = "Pa$$w0rd!"
        };

        var request = new HttpRequestMessage(HttpMethod.Post, "/User/AddUser")
        {
            Content = JsonContent.Create(addRequest)
        };
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);

        //Clean up
        context.User.Remove(adminUser);
        await context.SaveChangesAsync();
    }

    [Fact]
    public async Task AddUser_WithNameTooShort_ShouldReturnBadRequest()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
        var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

        var adminUser = await _helperService.CreateTestUserAsync(true);
        var token = tokenService.GenerateToken(adminUser.Id, ((RoleEnum)adminUser.RoleId).ToString());

        var addRequest = new AddUserRequest
        {
            Username = "newuser",
            Email = "newuser@example.com",
            Name = "",
            Surname = "NewSurname",
            RoleId = (int)RoleEnum.Employee,
            Password = "Pa$$w0rd!"
        };

        var request = new HttpRequestMessage(HttpMethod.Post, "/User/AddUser")
        {
            Content = JsonContent.Create(addRequest)
        };
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);

        //Clean up
        context.User.Remove(adminUser);
        await context.SaveChangesAsync();
    }

    [Fact]
    public async Task AddUser_WithNameTooLong_ShouldReturnBadRequest()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
        var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

        var adminUser = await _helperService.CreateTestUserAsync(true);
        var token = tokenService.GenerateToken(adminUser.Id, ((RoleEnum)adminUser.RoleId).ToString());

        var addRequest = new AddUserRequest
        {
            Username = "newuser",
            Email = "newuser@example.com",
            Name = new string('A', 101),
            Surname = "NewSurname",
            RoleId = (int)RoleEnum.Employee,
            Password = "Pa$$w0rd!"
        };

        var request = new HttpRequestMessage(HttpMethod.Post, "/User/AddUser")
        {
            Content = JsonContent.Create(addRequest)
        };
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);

        //Clean up
        context.User.Remove(adminUser);
        await context.SaveChangesAsync();
    }

    [Fact]
    public async Task AddUser_WithSurnameTooShort_ShouldReturnBadRequest()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
        var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

        var adminUser = await _helperService.CreateTestUserAsync(true);
        var token = tokenService.GenerateToken(adminUser.Id, ((RoleEnum)adminUser.RoleId).ToString());

        var addRequest = new AddUserRequest
        {
            Username = "newuser",
            Email = "newuser@example.com",
            Name = "NewName",
            Surname = "",
            RoleId = (int)RoleEnum.Employee,
            Password = "Pa$$w0rd!"
        };

        var request = new HttpRequestMessage(HttpMethod.Post, "/User/AddUser")
        {
            Content = JsonContent.Create(addRequest)
        };
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);

        //Clean up
        context.User.Remove(adminUser);
        await context.SaveChangesAsync();
    }

    [Fact]
    public async Task AddUser_WithSurnameTooLong_ShouldReturnBadRequest()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
        var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

        var adminUser = await _helperService.CreateTestUserAsync(true);
        var token = tokenService.GenerateToken(adminUser.Id, ((RoleEnum)adminUser.RoleId).ToString());

        var addRequest = new AddUserRequest
        {
            Username = "newuser",
            Email = "newuser@example.com",
            Name = "NewName",
            Surname = new string('A', 101),
            RoleId = (int)RoleEnum.Employee,
            Password = "Pa$$w0rd!"
        };

        var request = new HttpRequestMessage(HttpMethod.Post, "/User/AddUser")
        {
            Content = JsonContent.Create(addRequest)
        };
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);

        //Clean up
        context.User.Remove(adminUser);
        await context.SaveChangesAsync();
    }*/
}
