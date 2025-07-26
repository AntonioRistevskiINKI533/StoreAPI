using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using StoreAPI.Services;
using System.Net.Http;
using StoreAPI.Models.Contexts;
using StoreAPI.IntegrationTests.Shared;
using StoreAPI.Models.Datas;
using System.Net;
using StoreAPI.Enums;
using StoreAPI.Models;

namespace StoreAPI.IntegrationTests.UserController
{
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

            result.Items.Should().ContainSingle(u =>
                u.Id == testUser.Id &&
                u.Username == testUser.Username &&
                u.Email == testUser.Email &&
                u.Name == testUser.Name &&
                u.Surname == testUser.Surname &&
                u.RoleId == testUser.RoleId &&
                u.RoleName == ((RoleEnum)testUser.RoleId).ToString()
            );

            result.Items.Should().ContainSingle(u =>
                u.Id == anotherUser1.Id &&
                u.Username == anotherUser1.Username &&
                u.Email == anotherUser1.Email &&
                u.Name == anotherUser1.Name &&
                u.Surname == anotherUser1.Surname &&
                u.RoleId == anotherUser1.RoleId &&
                u.RoleName == ((RoleEnum)anotherUser1.RoleId).ToString()
            );

            result.Items.Should().ContainSingle(u =>
                u.Id == anotherUser2.Id &&
                u.Username == anotherUser2.Username &&
                u.Email == anotherUser2.Email &&
                u.Name == anotherUser2.Name &&
                u.Surname == anotherUser2.Surname &&
                u.RoleId == anotherUser2.RoleId &&
                u.RoleName == ((RoleEnum)anotherUser2.RoleId).ToString()
            );

            //Clean up
            context.User.Remove(testUser);
            context.User.Remove(anotherUser1);
            context.User.Remove(anotherUser2);
            await context.SaveChangesAsync();
        }

        [Fact]
        public async Task GetAllUsersPaged_PagingTest_ShouldReturnOkAndPagedResult()
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
            var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

            var testUser = await _helperService.CreateTestUserAsync();
            var token = tokenService.GenerateToken(testUser.Id, ((RoleEnum)testUser.RoleId).ToString());

            var anotherUser1 = await _helperService.CreateTestUserAsync(false, testUser.Name, testUser.Surname);
            var anotherUser2 = await _helperService.CreateTestUserAsync(false, testUser.Name, testUser.Surname);

            //using a filter in order to test the paging correctly, since other tests data interferes sometimes if all tests run at the same time
            var request = new HttpRequestMessage(HttpMethod.Get, $"/User/GetAllUsersPaged?pageIndex=1&pageSize=1&fullName={testUser.Name} {testUser.Surname}");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _client.SendAsync(request);

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<PagedModel<UserData>>();
            result.Should().NotBeNull();
            result.Items.Should().NotBeNull();
            result.Items.Count.Should().Be(1);

            result.Items.Should().ContainSingle(u =>
                u.Id == anotherUser1.Id &&
                u.Username == anotherUser1.Username &&
                u.Email == anotherUser1.Email &&
                u.Name == anotherUser1.Name &&
                u.Surname == anotherUser1.Surname &&
                u.RoleId == anotherUser1.RoleId &&
                u.RoleName == ((RoleEnum)anotherUser1.RoleId).ToString()
            );

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
            var token = tokenService.GenerateToken(testUser.Id, ((RoleEnum)testUser.RoleId).ToString());

            var anotherUser1 = await _helperService.CreateTestUserAsync();
            var anotherUser2 = await _helperService.CreateTestUserAsync();

            var request = new HttpRequestMessage(HttpMethod.Get, $"/User/GetAllUsersPaged?pageIndex=0&pageSize=10&fullName={testUser.Name}");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _client.SendAsync(request);

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<PagedModel<UserData>>();
            result.Should().NotBeNull();
            result.Items.Should().NotBeNull();
            result.Items.Count.Should().Be(1);

            result.Items.Should().ContainSingle(u =>
                u.Id == testUser.Id &&
                u.Username == testUser.Username &&
                u.Email == testUser.Email &&
                u.Name == testUser.Name &&
                u.Surname == testUser.Surname &&
                u.RoleId == testUser.RoleId &&
                u.RoleName == ((RoleEnum)testUser.RoleId).ToString()
            );

            //Clean up
            context.User.Remove(testUser);
            context.User.Remove(anotherUser1);
            context.User.Remove(anotherUser2);
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

            var employeeUser = await _helperService.CreateTestUserAsync();

            var request = new HttpRequestMessage(HttpMethod.Get, $"/User/GetAllUsersPaged?pageIndex=0&pageSize=10&roleId={employeeUser.RoleId}");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _client.SendAsync(request);

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<PagedModel<UserData>>();
            result.Should().NotBeNull();
            result.Items.Should().NotBeNull();
            result.Items.Count.Should().BeGreaterThanOrEqualTo(1);

            result.Items.Should().ContainSingle(u =>
                u.Id == employeeUser.Id &&
                u.Username == employeeUser.Username &&
                u.Email == employeeUser.Email &&
                u.Name == employeeUser.Name &&
                u.Surname == employeeUser.Surname &&
                u.RoleId == employeeUser.RoleId &&
                u.RoleName == ((RoleEnum)employeeUser.RoleId).ToString()
            );

            //Clean up
            context.User.Remove(adminUser);
            context.User.Remove(employeeUser);
            await context.SaveChangesAsync();
        }
    }
}