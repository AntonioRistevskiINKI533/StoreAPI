using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using StoreAPI.Services;
using System.Net.Http;
using StoreAPI.Models.Contexts;
using StoreAPI.IntegrationTests.Shared;
using StoreAPI.Enums;
using Microsoft.EntityFrameworkCore;

namespace StoreAPI.IntegrationTests.UserController
{
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
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
            var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

            var userToRemove = await _helperService.CreateTestUserAsync();

            var adminUser = await _helperService.CreateTestUserAsync(true);

            var adminToken = tokenService.GenerateToken(adminUser.Id, ((RoleEnum)adminUser.RoleId).ToString());

            var request = new HttpRequestMessage(HttpMethod.Delete, $"/User/RemoveUser?userId={userToRemove.Id}");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", adminToken);

            var response = await _client.SendAsync(request);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

            (await context.User.AnyAsync(c => c.Id == userToRemove.Id)).Should().BeFalse();

            //Clean up
            context.User.Remove(adminUser);
            await context.SaveChangesAsync();
        }

        [Fact]
        public async Task RemoveUser_WithNonExistentUserId_ShouldReturnNotFound()
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
            var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

            var userToRemove = await _helperService.CreateTestUserAsync();

            context.User.Remove(userToRemove);
            await context.SaveChangesAsync();

            var adminUser = await _helperService.CreateTestUserAsync(true);

            var adminToken = tokenService.GenerateToken(adminUser.Id, ((RoleEnum)adminUser.RoleId).ToString());

            var request = new HttpRequestMessage(HttpMethod.Delete, $"/User/RemoveUser?userId={userToRemove.Id}");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", adminToken);

            var response = await _client.SendAsync(request);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);

            var content = await response.Content.ReadAsStringAsync();
            content.Should().Be("User does not exist");

            //Clean up
            context.User.Remove(adminUser);
            await context.SaveChangesAsync();
        }
    }
}