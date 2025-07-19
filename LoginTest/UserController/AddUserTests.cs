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
            Username = "newuser",//TODO
            Email = _helperService.CreateRandomEmail(),
            Name = _helperService.CreateRandomText(),
            Surname = _helperService.CreateRandomText(),
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
        context.User.Remove(addedUser);
        await context.SaveChangesAsync();
    }

    [Fact]
    public async Task AddUser_WithEmployeeToken_ShouldReturnForbidden()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
        var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

        var employeeUser = await _helperService.CreateTestUserAsync();
        var token = tokenService.GenerateToken(employeeUser.Id, ((RoleEnum)employeeUser.RoleId).ToString());

        var addRequest = new AddUserRequest
        {
            Username = "newuser",
            Email = _helperService.CreateRandomEmail(),
            Name = _helperService.CreateRandomText(),
            Surname = _helperService.CreateRandomText(),
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
            Email = _helperService.CreateRandomEmail(),
            Name = _helperService.CreateRandomText(),
            Surname = _helperService.CreateRandomText(),
            RoleId = (int)RoleEnum.Employee,
            Password = "Pa$$w0rd!"
        };

        var request = new HttpRequestMessage(HttpMethod.Post, "/User/AddUser")
        {
            Content = JsonContent.Create(addRequest)
        };
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Conflict);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().Be("User with same username already exists");

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
            Name = _helperService.CreateRandomText(),
            Surname = _helperService.CreateRandomText(),
            RoleId = (int)RoleEnum.Employee,
            Password = "Pa$$w0rd!"
        };

        var request = new HttpRequestMessage(HttpMethod.Post, "/User/AddUser")
        {
            Content = JsonContent.Create(addRequest)
        };
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Conflict);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().Be("User with same email already exists");

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
            Name = _helperService.CreateRandomText(),
            Surname = _helperService.CreateRandomText(),
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

        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Invalid email");

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
            Email = _helperService.CreateRandomEmail(),
            Name = _helperService.CreateRandomText(),
            Surname = _helperService.CreateRandomText(),
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

        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Password must be at least 8 characters");

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
            Email = _helperService.CreateRandomEmail(),
            Name = _helperService.CreateRandomText(),
            Surname = _helperService.CreateRandomText(),
            RoleId = 9999, //non-existent role
            Password = "Pa$$w0rd!"
        };

        var request = new HttpRequestMessage(HttpMethod.Post, "/User/AddUser")
        {
            Content = JsonContent.Create(addRequest)
        };
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().Be("Role does not exist");

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
            Email = _helperService.CreateRandomEmail(),
            Name = _helperService.CreateRandomText(),
            Surname = _helperService.CreateRandomText(),
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

        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Username must be between 5 and 20 characters");

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
            Email = _helperService.CreateRandomEmail(),
            Name = _helperService.CreateRandomText(),
            Surname = _helperService.CreateRandomText(),
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

        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Username must be between 5 and 20 characters");

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
            Email = _helperService.CreateRandomEmail(),
            Name = "",
            Surname = _helperService.CreateRandomText(),
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

        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Name must be between 1 and 100 characters");

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
            Email = _helperService.CreateRandomEmail(),
            Name = new string('A', 101),
            Surname = _helperService.CreateRandomText(),
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

        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Name must be between 1 and 100 characters");

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
            Email = _helperService.CreateRandomEmail(),
            Name = _helperService.CreateRandomText(),
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

        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Surname must be between 1 and 100 characters");

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
            Email = _helperService.CreateRandomEmail(),
            Name = _helperService.CreateRandomText(),
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

        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Surname must be between 1 and 100 characters");

        //Clean up
        context.User.Remove(adminUser);
        await context.SaveChangesAsync();
    }
}
