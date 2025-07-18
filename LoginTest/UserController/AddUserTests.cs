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

public class AddUserIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;
    private readonly HelperService _helperService;

    public AddUserIntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
        _helperService = new HelperService(_factory);
    }

    [Fact]
    public async Task AddUser_WithAdminTokenAndValidData_ShouldReturnOk()
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
            Password = "Pa$$w0rd!"
        };

        var request = new HttpRequestMessage(HttpMethod.Post, "/User/AddUser")
        {
            Content = JsonContent.Create(addRequest)
        };
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

        var addedUser = await context.User.FirstOrDefaultAsync(u => u.Username == addRequest.Username);
        addedUser.Should().NotBeNull();
        addedUser.Username.Should().Be(addRequest.Username);
        addedUser.Email.Should().Be(addRequest.Email);
        addedUser.Name.Should().Be(addRequest.Name);
        addedUser.Surname.Should().Be(addRequest.Surname);
        addedUser.RoleId.Should().Be(addRequest.RoleId);

        var loginRequest = new LoginRequest
        {
            Username = addedUser.Username,
            Password = addRequest.Password
        };

        //Test if user can log in with the created credentials
        var loginResponse = await _client.PostAsJsonAsync("/User/Login", loginRequest);

        loginResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

        //Clean up
        context.User.Remove(adminUser);
        context.User.Remove(addedUser!);
        await context.SaveChangesAsync();
    }

    [Fact]
    public async Task AddUser_WithEmployeeToken_ShouldReturnForbidden()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
        var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

        var employeeUser = await _helperService.CreateTestUserAsync(isAdmin: false);
        var token = tokenService.GenerateToken(employeeUser.Id, ((RoleEnum)employeeUser.RoleId).ToString());

        var addRequest = new AddUserRequest
        {
            Username = "newuser",
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

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Forbidden);

        //Clean up
        context.User.Remove(employeeUser);
        await context.SaveChangesAsync();
    }

    [Fact]
    public async Task AddUser_WithExistingUsername_ShouldReturnBadRequest()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
        var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

        var adminUser = await _helperService.CreateTestUserAsync(true);
        var existingUser = await _helperService.CreateTestUserAsync();

        var token = tokenService.GenerateToken(adminUser.Id, ((RoleEnum)adminUser.RoleId).ToString());

        var addRequest = new AddUserRequest
        {
            Username = existingUser.Username, //duplicate username
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
        context.User.Remove(existingUser);
        await context.SaveChangesAsync();
    }

    [Fact]
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
    }
}
