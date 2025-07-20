using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using StoreAPI.Services;
using System.Net.Http;
using StoreAPI.Models.Contexts;
using StoreAPI.Models.Datas;
using StoreAPI.IntegrationTests.Shared;
using StoreAPI.Enums;

namespace StoreAPI.IntegrationTests.CompanyController
{
    public class GetCompanyIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory _factory;
        private readonly HelperService _helperService;

        public GetCompanyIntegrationTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
            _helperService = new HelperService(_factory);
        }

        [Fact]
        public async Task GetCompany_WithValidCompanyIdAndToken_ShouldReturnOk()
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
            var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

            var testUser = await _helperService.CreateTestUserAsync();

            var token = tokenService.GenerateToken(testUser.Id, ((RoleEnum)testUser.RoleId).ToString());

            var testCompany = await _helperService.CreateTestCompanyAsync();

            var request = new HttpRequestMessage(HttpMethod.Get, $"/Company/GetCompany?companyId={testCompany.Id}");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _client.SendAsync(request);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

            var companyData = await response.Content.ReadFromJsonAsync<CompanyData>();
            companyData.Should().NotBeNull();
            companyData!.Id.Should().Be(testCompany.Id);
            companyData.Name.Should().Be(testCompany.Name);
            companyData.Address.Should().Be(testCompany.Address);
            companyData.Phone.Should().Be(testCompany.Phone);

            //Cleanup
            context.User.Remove(testUser);
            context.Company.Remove(testCompany);
            await context.SaveChangesAsync();
        }

        [Fact]
        public async Task GetCompany_WithNonExistentCompanyId_ShouldReturnNotFound()
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
            var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

            var testUser = await _helperService.CreateTestUserAsync();

            var token = tokenService.GenerateToken(testUser.Id, ((RoleEnum)testUser.RoleId).ToString());

            var testCompany = await _helperService.CreateTestCompanyAsync();

            context.Company.Remove(testCompany);
            await context.SaveChangesAsync();

            var request = new HttpRequestMessage(HttpMethod.Get, $"/Company/GetCompany?companyId={testCompany.Id}");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _client.SendAsync(request);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);

            var content = await response.Content.ReadAsStringAsync();
            content.Should().Be("Company does not exist");

            //Cleanup
            context.User.Remove(testUser);
            await context.SaveChangesAsync();
        }
    }
}