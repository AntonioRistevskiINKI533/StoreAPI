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

namespace StoreAPI.IntegrationTests.CompanyController
{
    public class GetAllCompaniesPagedIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory _factory;
        private readonly HelperService _helperService;

        public GetAllCompaniesPagedIntegrationTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
            _helperService = new HelperService(_factory);
        }

        [Fact]
        public async Task GetAllCompaniesPaged_WithValidToken_ShouldReturnOkAndPagedResult()
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
            var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

            var testUser = await _helperService.CreateTestUserAsync();

            var token = tokenService.GenerateToken(testUser.Id, ((RoleEnum)testUser.RoleId).ToString());

            //using a filter in order to test the paging correctly, since other tests data (companies) interferes sometimes if all tests run at the same time
            string companyNamePrefix = "pagetestcomp";

            var company1 = await _helperService.CreateTestCompanyAsync(companyNamePrefix);
            var company2 = await _helperService.CreateTestCompanyAsync(companyNamePrefix);

            var request = new HttpRequestMessage(HttpMethod.Get, $"/Company/GetAllCompaniesPaged?pageIndex=0&pageSize=10&name={companyNamePrefix}");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _client.SendAsync(request);

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<PagedModel<CompanyData>>();
            result.Should().NotBeNull();
            result.Items.Should().NotBeNull();
            result.Items.Count.Should().BeGreaterThanOrEqualTo(2); //should contain at least the two created companies

            result.Items.Should().ContainSingle(c =>
                c.Id == company1.Id &&
                c.Name == company1.Name &&
                c.Address == company1.Address &&
                c.Phone == company1.Phone
            );

            result.Items.Should().ContainSingle(c =>
                c.Id == company2.Id &&
                c.Name == company2.Name &&
                c.Address == company2.Address &&
                c.Phone == company2.Phone
            );

            //Clean up
            context.User.Remove(testUser);
            context.Company.Remove(company1);
            context.Company.Remove(company2);
            await context.SaveChangesAsync();
        }

        [Fact]
        public async Task GetAllCompaniesPaged_PagingTest_ShouldReturnOkAndPagedResult()
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
            var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

            var testUser = await _helperService.CreateTestUserAsync();

            var token = tokenService.GenerateToken(testUser.Id, ((RoleEnum)testUser.RoleId).ToString());

            string companyNamePrefix = "pagetestcomp";

            var company1 = await _helperService.CreateTestCompanyAsync(companyNamePrefix);
            var company2 = await _helperService.CreateTestCompanyAsync(companyNamePrefix);
            var company3 = await _helperService.CreateTestCompanyAsync(companyNamePrefix);

            var request = new HttpRequestMessage(HttpMethod.Get, $"/Company/GetAllCompaniesPaged?pageIndex=2&pageSize=1&name={companyNamePrefix}");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _client.SendAsync(request);

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<PagedModel<CompanyData>>();
            result.Should().NotBeNull();
            result.Items.Should().NotBeNull();
            result.Items.Count.Should().Be(1);

            result.Items.Should().ContainSingle(c =>
                c.Id == company3.Id &&
                c.Name == company3.Name &&
                c.Address == company3.Address &&
                c.Phone == company3.Phone
            );

            //Clean up
            context.User.Remove(testUser);
            context.Company.Remove(company1);
            context.Company.Remove(company2);
            context.Company.Remove(company3);
            await context.SaveChangesAsync();
        }
    }
}