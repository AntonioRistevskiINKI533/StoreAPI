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
using System.Linq;
using System;

namespace StoreAPI.IntegrationTests.ProductController
{
    public class GetAllProductsPagedIntegrationTests : IClassFixture<CustomWebApplicationFactory>, IDisposable
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory _factory;
        private readonly HelperService _helperService;
        private readonly string prefix = "GetAllProductsPaged_";

        public GetAllProductsPagedIntegrationTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
            _helperService = new HelperService(_factory);
        }

        [Fact]
        public async Task GetAllProductsPaged_WithValidToken_ShouldReturnOkAndPagedResult()
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
            var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

            var testUser = await _helperService.CreateTestUserAsync(prefix);

            var token = tokenService.GenerateToken(testUser.Id, ((RoleEnum)testUser.RoleId).ToString());

            var company = await _helperService.CreateTestCompanyAsync(prefix);

            var product1 = await _helperService.CreateTestProductAsync(company.Id);
            var product2 = await _helperService.CreateTestProductAsync(company.Id);

            var request = new HttpRequestMessage(HttpMethod.Get, "/Product/GetAllProductsPaged?pageIndex=0&pageSize=10");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _client.SendAsync(request);

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<PagedModel<ProductData>>();
            result.Should().NotBeNull();
            result.Items.Should().NotBeNull();
            result.Items.Count.Should().BeGreaterThanOrEqualTo(2); //should contain at least the two created products

            result.Items.Should().ContainSingle(p =>
                p.Id == product1.Id &&
                p.RegistrationNumber == product1.RegistrationNumber &&
                p.Name == product1.Name &&
                p.CompanyId == product1.CompanyId &&
                p.Price == product1.Price &&
                p.CompanyName == company.Name
            );

            result.Items.Should().ContainSingle(p =>
                p.Id == product2.Id &&
                p.RegistrationNumber == product2.RegistrationNumber &&
                p.Name == product2.Name &&
                p.CompanyId == product2.CompanyId &&
                p.Price == product2.Price &&
                p.CompanyName == company.Name
            );
        }

        [Fact]
        public async Task GetAllProductsPaged_PagingTest_ShouldReturnOkAndPagedResult()
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
            var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

            var testUser = await _helperService.CreateTestUserAsync(prefix);

            var token = tokenService.GenerateToken(testUser.Id, ((RoleEnum)testUser.RoleId).ToString());

            var company = await _helperService.CreateTestCompanyAsync(prefix);

            var product1 = await _helperService.CreateTestProductAsync(company.Id);
            var product2 = await _helperService.CreateTestProductAsync(company.Id);
            var product3 = await _helperService.CreateTestProductAsync(company.Id);
            var product4 = await _helperService.CreateTestProductAsync(company.Id);
            var product5 = await _helperService.CreateTestProductAsync(company.Id);

            //using a filter in order to test the paging correctly, since other tests data interferes sometimes if all tests run at the same time
            var request = new HttpRequestMessage(HttpMethod.Get, $"/Product/GetAllProductsPaged?pageIndex=2&pageSize=2&companyId={company.Id}");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _client.SendAsync(request);

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<PagedModel<ProductData>>();
            result.Should().NotBeNull();
            result.Items.Should().NotBeNull();
            result.Items.Count.Should().BeGreaterThanOrEqualTo(1);

            result.Items.Should().ContainSingle(p =>
                p.Id == product5.Id &&
                p.RegistrationNumber == product5.RegistrationNumber &&
                p.Name == product5.Name &&
                p.CompanyId == product5.CompanyId &&
                p.Price == product5.Price &&
                p.CompanyName == company.Name
            );
        }

        [Fact]
        public async Task GetAllProductsPaged_WithProductNameFilter_ShouldReturnFilteredResults()
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
            var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

            var testUser = await _helperService.CreateTestUserAsync(prefix);

            var token = tokenService.GenerateToken(testUser.Id, ((RoleEnum)testUser.RoleId).ToString());

            var company = await _helperService.CreateTestCompanyAsync(prefix);

            var product1 = await _helperService.CreateTestProductAsync(company.Id);
            var product2 = await _helperService.CreateTestProductAsync(company.Id);
            var product3 = await _helperService.CreateTestProductAsync(company.Id);

            var request = new HttpRequestMessage(HttpMethod.Get, $"/Product/GetAllProductsPaged?pageIndex=0&pageSize=10&productName={product3.Name}");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _client.SendAsync(request);

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<PagedModel<ProductData>>();
            result.Should().NotBeNull();
            result.Items.Should().NotBeNull();
            result.Items.Count.Should().Be(1);

            result.Items.Should().ContainSingle(p =>
                p.Id == product3.Id &&
                p.RegistrationNumber == product3.RegistrationNumber &&
                p.Name == product3.Name &&
                p.CompanyId == product3.CompanyId &&
                p.Price == product3.Price &&
                p.CompanyName == company.Name
            );
        }

        [Fact]
        public async Task GetAllProductsPaged_WithCompanyIdFilter_ShouldReturnFilteredResults()
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
            var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

            var testUser = await _helperService.CreateTestUserAsync(prefix);

            var token = tokenService.GenerateToken(testUser.Id, ((RoleEnum)testUser.RoleId).ToString());

            var company1 = await _helperService.CreateTestCompanyAsync(prefix);
            var company2 = await _helperService.CreateTestCompanyAsync(prefix);

            var product1 = await _helperService.CreateTestProductAsync(company1.Id);
            var product2 = await _helperService.CreateTestProductAsync(company2.Id);
            var product3 = await _helperService.CreateTestProductAsync(company2.Id);

            var request = new HttpRequestMessage(HttpMethod.Get, $"/Product/GetAllProductsPaged?pageIndex=0&pageSize=10&companyId={company2.Id}");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _client.SendAsync(request);

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<PagedModel<ProductData>>();
            result.Should().NotBeNull();
            result.Items.Should().NotBeNull();
            result.Items.Count.Should().Be(2);

            result.Items.Should().ContainSingle(p =>
                p.Id == product2.Id &&
                p.RegistrationNumber == product2.RegistrationNumber &&
                p.Name == product2.Name &&
                p.CompanyId == product2.CompanyId &&
                p.Price == product2.Price &&
                p.CompanyName == company2.Name
            );

            result.Items.Should().ContainSingle(p =>
                p.Id == product3.Id &&
                p.RegistrationNumber == product3.RegistrationNumber &&
                p.Name == product3.Name &&
                p.CompanyId == product3.CompanyId &&
                p.Price == product3.Price &&
                p.CompanyName == company2.Name
            );
        }

        public void Dispose()
        {
            _helperService.CleanUp(prefix);
        }
    }
}