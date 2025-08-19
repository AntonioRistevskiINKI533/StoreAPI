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
using System;
using System.Linq;

namespace StoreAPI.IntegrationTests.CompanyController
{
    public class RemoveCompanyIntegrationTests : IClassFixture<CustomWebApplicationFactory>, IDisposable
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory _factory;
        private readonly HelperService _helperService;
        private readonly string prefix = "RemoveCompany_";

        public RemoveCompanyIntegrationTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
            _helperService = new HelperService(_factory);
        }

        [Fact]
        public async Task RemoveCompany_WithValidCompanyIdAndDeletableCompany_ShouldReturnOk()
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
            var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

            var testUser = await _helperService.CreateTestUserAsync(prefix);

            var token = tokenService.GenerateToken(testUser.Id, ((RoleEnum)testUser.RoleId).ToString());

            var testCompany = await _helperService.CreateTestCompanyAsync(prefix);

            var request = new HttpRequestMessage(HttpMethod.Delete, $"/Company/RemoveCompany?companyId={testCompany.Id}");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _client.SendAsync(request);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

            (await context.Company.AnyAsync(c => c.Id == testCompany.Id)).Should().BeFalse();
        }

        [Fact]
        public async Task RemoveCompany_WithNonExistentCompanyId_ShouldReturnNotFound()
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
            var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

            var testUser = await _helperService.CreateTestUserAsync(prefix);

            var token = tokenService.GenerateToken(testUser.Id, ((RoleEnum)testUser.RoleId).ToString());

            var testCompany = await _helperService.CreateTestCompanyAsync(prefix);

            context.Company.Remove(testCompany);
            await context.SaveChangesAsync();

            var request = new HttpRequestMessage(HttpMethod.Delete, $"/Company/RemoveCompany?companyId={testCompany.Id}");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _client.SendAsync(request);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);

            var content = await response.Content.ReadAsStringAsync();
            content.Should().Be("Company does not exist");
        }

        [Fact]
        public async Task RemoveCompany_WithExistingProduct_ShouldReturnConflict()
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
            var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

            var testUser = await _helperService.CreateTestUserAsync(prefix);

            var token = tokenService.GenerateToken(testUser.Id, ((RoleEnum)testUser.RoleId).ToString());

            var testCompany = await _helperService.CreateTestCompanyAsync(prefix);

            var testProduct = await _helperService.CreateTestProductAsync(testCompany.Id);

            var request = new HttpRequestMessage(HttpMethod.Delete, $"/Company/RemoveCompany?companyId={testCompany.Id}");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _client.SendAsync(request);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Conflict);

            var content = await response.Content.ReadAsStringAsync();
            content.Should().Be("Company has products, please delete them first");

            (await context.Company.AnyAsync(c => c.Id == testCompany.Id)).Should().BeTrue();
        }

        public void Dispose()
        {
            _helperService.CleanUp(prefix);
        }
    }
}