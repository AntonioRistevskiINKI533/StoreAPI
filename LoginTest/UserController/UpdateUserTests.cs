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
using StoreAPI.Enums;
using System;

public class UpdateUserIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;
    private readonly HelperService _helperService;

    public UpdateUserIntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
        _helperService = new HelperService(_factory);
    }

    [Fact]
    public async Task UpdateUser_WithAdminTokenAndValidData_ShouldReturnOk()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
        var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

        var adminUser = await _helperService.CreateTestUserAsync(true);

        var testUser = await _helperService.CreateTestUserAsync();

        var token = tokenService.GenerateToken(adminUser.Id, ((RoleEnum)adminUser.RoleId).ToString());

        var updateRequest = new UpdateUserRequest
        {
            Id = testUser.Id,
            Username = "updatedusername",
            Email = "updatedemail@example.com",
            Name = "UpdatedName",
            Surname = "UpdatedSurname",
            RoleId = (int)RoleEnum.Employee
        };

        var request = new HttpRequestMessage(HttpMethod.Put, "/User/UpdateUser")
        {
            Content = JsonContent.Create(updateRequest)
        };
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

        var updatedUser = await context.User.FindAsync(testUser.Id);
        updatedUser.Should().NotBeNull();
        updatedUser.Username.Should().Be(updateRequest.Username);
        updatedUser.Email.Should().Be(updateRequest.Email);
        updatedUser.Name.Should().Be(updateRequest.Name);
        updatedUser.Surname.Should().Be(updateRequest.Surname);
        updatedUser.RoleId.Should().Be(updateRequest.RoleId);

        //Clean up
        context.User.Remove(adminUser);
        context.User.Remove(updatedUser);
        await context.SaveChangesAsync();
    }

    [Fact]
    public async Task UpdateUser_WithoutToken_ShouldReturnUnauthorized()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<StoreContext>();

        var testUser = await _helperService.CreateTestUserAsync();

        var updateRequest = new UpdateUserRequest
        {
            Id = testUser.Id,
            Username = "updatedusername",
            Email = "updatedemail@example.com",
            Name = "UpdatedName",
            Surname = "UpdatedSurname",
            RoleId = (int)RoleEnum.Employee
        };

        var response = await _client.PutAsJsonAsync("/User/UpdateUser", updateRequest);

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);

        //Clean up
        context.User.Remove(testUser);
        await context.SaveChangesAsync();
    }

    [Fact]
    public async Task UpdateUser_WithEmployeeToken_ShouldReturnForbidden()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
        var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

        var employeeUser = await _helperService.CreateTestUserAsync();

        var testUser = await _helperService.CreateTestUserAsync();

        var token = tokenService.GenerateToken(employeeUser.Id, ((RoleEnum)employeeUser.RoleId).ToString());

        var updateRequest = new UpdateUserRequest
        {
            Id = testUser.Id,
            Username = "updatedusername",
            Email = "updatedemail@example.com",
            Name = "UpdatedName",
            Surname = "UpdatedSurname",
            RoleId = (int)RoleEnum.Employee
        };

        var request = new HttpRequestMessage(HttpMethod.Put, "/User/UpdateUser")
        {
            Content = JsonContent.Create(updateRequest)
        };
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Forbidden);

        //Clean up
        context.User.Remove(employeeUser);
        context.User.Remove(testUser);
        await context.SaveChangesAsync();
    }

    [Fact]
    public async Task UpdateUser_WithExistingUsername_ShouldReturnBadRequest()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
        var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

        var adminUser = await _helperService.CreateTestUserAsync(true);

        var testUser = await _helperService.CreateTestUserAsync();

        var anotherUser = await _helperService.CreateTestUserAsync();

        var token = tokenService.GenerateToken(adminUser.Id, ((RoleEnum)adminUser.RoleId).ToString());

        var updateRequest = new UpdateUserRequest
        {
            Id = testUser.Id,
            Username = anotherUser.Username,
            Email = "updatedemail@example.com",
            Name = "UpdatedName",
            Surname = "UpdatedSurname",
            RoleId = (int)RoleEnum.Employee
        };

        var request = new HttpRequestMessage(HttpMethod.Put, "/User/UpdateUser")
        {
            Content = JsonContent.Create(updateRequest)
        };
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);

        //Clean up
        context.User.Remove(adminUser);
        context.User.Remove(testUser);
        context.User.Remove(anotherUser);
        await context.SaveChangesAsync();
    }

    [Fact]
    public async Task UpdateUser_WithExistingEmail_ShouldReturnBadRequest()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
        var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

        var adminUser = await _helperService.CreateTestUserAsync(true);

        var testUser = await _helperService.CreateTestUserAsync();

        var anotherUser = await _helperService.CreateTestUserAsync();

        var token = tokenService.GenerateToken(adminUser.Id, ((RoleEnum)adminUser.RoleId).ToString());

        var updateRequest = new UpdateUserRequest
        {
            Id = testUser.Id,
            Username = "updatedusername",
            Email = anotherUser.Email,
            Name = "UpdatedName",
            Surname = "UpdatedSurname",
            RoleId = (int)RoleEnum.Employee
        };

        var request = new HttpRequestMessage(HttpMethod.Put, "/User/UpdateUser")
        {
            Content = JsonContent.Create(updateRequest)
        };
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);

        //Clean up
        context.User.Remove(adminUser);
        context.User.Remove(testUser);
        context.User.Remove(anotherUser);
        await context.SaveChangesAsync();
    }

    [Fact]
    public async Task UpdateUser_WithInvalidEmailFormat_ShouldReturnBadRequest()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
        var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

        var adminUser = await _helperService.CreateTestUserAsync(true);

        var testUser = await _helperService.CreateTestUserAsync();

        var token = tokenService.GenerateToken(adminUser.Id, ((RoleEnum)adminUser.RoleId).ToString());

        var updateRequest = new UpdateUserRequest
        {
            Id = testUser.Id,
            Username = "updatedusername",
            Email = "invalid-email-format", //invalid email
            Name = "UpdatedName",
            Surname = "UpdatedSurname",
            RoleId = (int)RoleEnum.Employee
        };

        var request = new HttpRequestMessage(HttpMethod.Put, "/User/UpdateUser")
        {
            Content = JsonContent.Create(updateRequest)
        };
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);

        //Clean up
        context.User.Remove(adminUser);
        context.User.Remove(testUser);
        await context.SaveChangesAsync();
    }

    [Fact]
    public async Task UpdateUser_WithUsernameTooShort_ShouldReturnBadRequest()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
        var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

        var adminUser = await _helperService.CreateTestUserAsync(true);

        var testUser = await _helperService.CreateTestUserAsync();

        var token = tokenService.GenerateToken(adminUser.Id, ((RoleEnum)adminUser.RoleId).ToString());

        var updateRequest = new UpdateUserRequest
        {
            Id = testUser.Id,
            Username = "abc", //too short (min length = 5)
            Email = "updatedemail@example.com",
            Name = "UpdatedName",
            Surname = "UpdatedSurname",
            RoleId = (int)RoleEnum.Employee
        };

        var request = new HttpRequestMessage(HttpMethod.Put, "/User/UpdateUser")
        {
            Content = JsonContent.Create(updateRequest)
        };
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);

        //Clean up
        context.User.Remove(adminUser);
        context.User.Remove(testUser);
        await context.SaveChangesAsync();
    }

    [Fact]
    public async Task UpdateUser_WithUsernameTooLong_ShouldReturnBadRequest()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
        var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

        var adminUser = await _helperService.CreateTestUserAsync(true);

        var testUser = await _helperService.CreateTestUserAsync();

        var token = tokenService.GenerateToken(adminUser.Id, ((RoleEnum)adminUser.RoleId).ToString());

        var updateRequest = new UpdateUserRequest
        {
            Id = testUser.Id,
            Username = new string('A', 21), //too long
            Email = "updatedemail@example.com",
            Name = "UpdatedName",
            Surname = "UpdatedSurname",
            RoleId = (int)RoleEnum.Employee
        };

        var request = new HttpRequestMessage(HttpMethod.Put, "/User/UpdateUser")
        {
            Content = JsonContent.Create(updateRequest)
        };
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);

        //Clean up
        context.User.Remove(adminUser);
        context.User.Remove(testUser);
        await context.SaveChangesAsync();
    }

    [Fact]
    public async Task UpdateUser_WithNameTooShort_ShouldReturnBadRequest()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
        var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

        var adminUser = await _helperService.CreateTestUserAsync(true);

        var testUser = await _helperService.CreateTestUserAsync();

        var token = tokenService.GenerateToken(adminUser.Id, ((RoleEnum)adminUser.RoleId).ToString());

        var updateRequest = new UpdateUserRequest
        {
            Id = testUser.Id,
            Username = "updatedusername",
            Email = "updatedemail@example.com",
            Name = "",
            Surname = "UpdatedSurname",
            RoleId = (int)RoleEnum.Employee
        };

        var request = new HttpRequestMessage(HttpMethod.Put, "/User/UpdateUser")
        {
            Content = JsonContent.Create(updateRequest)
        };
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);

        //Clean up
        context.User.Remove(adminUser);
        context.User.Remove(testUser);
        await context.SaveChangesAsync();
    }

    [Fact]
    public async Task UpdateUser_WithNameTooLong_ShouldReturnBadRequest()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
        var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

        var adminUser = await _helperService.CreateTestUserAsync(true);

        var testUser = await _helperService.CreateTestUserAsync();

        var token = tokenService.GenerateToken(adminUser.Id, ((RoleEnum)adminUser.RoleId).ToString());

        var updateRequest = new UpdateUserRequest
        {
            Id = testUser.Id,
            Username = "updatedusername",
            Email = "updatedemail@example.com",
            Name = new string('A', 101), // too long (max length = 100)
            Surname = "UpdatedSurname",
            RoleId = (int)RoleEnum.Employee
        };

        var request = new HttpRequestMessage(HttpMethod.Put, "/User/UpdateUser")
        {
            Content = JsonContent.Create(updateRequest)
        };
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);

        //Clean up
        context.User.Remove(adminUser);
        context.User.Remove(testUser);
        await context.SaveChangesAsync();
    }

    [Fact]
    public async Task UpdateUser_WithSurnameTooShort_ShouldReturnBadRequest()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
        var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

        var adminUser = await _helperService.CreateTestUserAsync(true);

        var testUser = await _helperService.CreateTestUserAsync();

        var token = tokenService.GenerateToken(adminUser.Id, ((RoleEnum)adminUser.RoleId).ToString());

        var updateRequest = new UpdateUserRequest
        {
            Id = testUser.Id,
            Username = "updatedusername",
            Email = "updatedemail@example.com",
            Name = "UpdateName",
            Surname = "", //too short
            RoleId = (int)RoleEnum.Employee
        };

        var request = new HttpRequestMessage(HttpMethod.Put, "/User/UpdateUser")
        {
            Content = JsonContent.Create(updateRequest)
        };
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);

        //Clean up
        context.User.Remove(adminUser);
        context.User.Remove(testUser);
        await context.SaveChangesAsync();
    }

    [Fact]
    public async Task UpdateUser_WithSurnameTooLong_ShouldReturnBadRequest()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
        var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

        var adminUser = await _helperService.CreateTestUserAsync(true);

        var testUser = await _helperService.CreateTestUserAsync();

        var token = tokenService.GenerateToken(adminUser.Id, ((RoleEnum)adminUser.RoleId).ToString());

        var updateRequest = new UpdateUserRequest
        {
            Id = testUser.Id,
            Username = "updatedusername",
            Email = "updatedemail@example.com",
            Name = "UpdateName",
            Surname = new string('A', 101), //too long
            RoleId = (int)RoleEnum.Employee
        };

        var request = new HttpRequestMessage(HttpMethod.Put, "/User/UpdateUser")
        {
            Content = JsonContent.Create(updateRequest)
        };
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);

        //Clean up
        context.User.Remove(adminUser);
        context.User.Remove(testUser);
        await context.SaveChangesAsync();
    }

    [Fact]
    public async Task UpdateUser_WithNonExistentRoleId_ShouldReturnBadRequest()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
        var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

        var adminUser = await _helperService.CreateTestUserAsync(true);

        var testUser = await _helperService.CreateTestUserAsync();

        var token = tokenService.GenerateToken(adminUser.Id, ((RoleEnum)adminUser.RoleId).ToString());

        var updateRequest = new UpdateUserRequest
        {
            Id = testUser.Id,
            Username = "updatedusername",
            Email = "updatedemail@example.com",
            Name = "UpdatedName",
            Surname = "UpdatedSurname",
            RoleId = 9999 //Non-existent role ID
        };

        var request = new HttpRequestMessage(HttpMethod.Put, "/User/UpdateUser")
        {
            Content = JsonContent.Create(updateRequest)
        };
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);

        //Clean up
        context.User.Remove(adminUser);
        context.User.Remove(testUser);
        await context.SaveChangesAsync();
    }
}
