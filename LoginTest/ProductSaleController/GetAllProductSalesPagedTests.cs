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

namespace StoreAPI.IntegrationTests.ProductSaleController
{
    public class GetAllProductSalesPagedIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory _factory;
        private readonly HelperService _helperService;

        public GetAllProductSalesPagedIntegrationTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
            _helperService = new HelperService(_factory);
        }

        [Fact]
        public async Task GetAllProductSalesPaged_WithValidToken_ShouldReturnOkAndPagedResult()
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
            var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

            var testUser = await _helperService.CreateTestUserAsync();

            var token = tokenService.GenerateToken(testUser.Id, ((RoleEnum)testUser.RoleId).ToString());

            var company = await _helperService.CreateTestCompanyAsync();

            var product = await _helperService.CreateTestProductAsync(company.Id);

            var productSale1 = await _helperService.CreateTestProductSaleAsync(product.Id);
            var productSale2 = await _helperService.CreateTestProductSaleAsync(product.Id);

            var request = new HttpRequestMessage(HttpMethod.Get, "/ProductSale/GetAllProductSalesPaged?pageIndex=0&pageSize=10");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _client.SendAsync(request);

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<PagedModel<ProductSaleData>>();
            result.Should().NotBeNull();
            result.Items.Should().NotBeNull();
            result.Items.Count.Should().BeGreaterThanOrEqualTo(2); //should contain at least the two created products

            result.Items.Should().ContainSingle(p =>
                p.Id == productSale1.Id &&
                p.ProductId == productSale1.ProductId &&
                p.SoldAmount == productSale1.SoldAmount &&
                p.PricePerUnit == productSale1.PricePerUnit &&
                p.Date.ToString() == productSale1.Date.ToString() &&
                p.ProductName ==product.Name
            );

            result.Items.Should().ContainSingle(p =>
                p.Id == productSale2.Id &&
                p.ProductId == productSale2.ProductId &&
                p.SoldAmount == productSale2.SoldAmount &&
                p.PricePerUnit == productSale2.PricePerUnit &&
                p.Date.ToString() == productSale2.Date.ToString() &&
                p.ProductName == product.Name
            );

            //Clean up
            context.ProductSale.Remove(productSale1);
            context.ProductSale.Remove(productSale2);
            await context.SaveChangesAsync();

            context.Product.Remove(product);
            await context.SaveChangesAsync();

            context.User.Remove(testUser);
            context.Company.Remove(company);
            await context.SaveChangesAsync();
        }

/*        [Fact]
        public async Task GetAllProductsPaged_WithProductNameFilter_ShouldReturnFilteredResults()
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
            var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

            var testUser = await _helperService.CreateTestUserAsync();

            var token = tokenService.GenerateToken(testUser.Id, ((RoleEnum)testUser.RoleId).ToString());

            var company = await _helperService.CreateTestCompanyAsync();

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
                p.Price == product3.Price
            );

            //Clean up
            context.Product.Remove(product1);
            context.Product.Remove(product2);
            context.Product.Remove(product3);
            await context.SaveChangesAsync();

            context.User.Remove(testUser);
            context.Company.Remove(company);
            await context.SaveChangesAsync();
        }

        [Fact]
        public async Task GetAllProductsPaged_WithCompanyIdFilter_ShouldReturnFilteredResults()
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
            var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

            var testUser = await _helperService.CreateTestUserAsync();

            var token = tokenService.GenerateToken(testUser.Id, ((RoleEnum)testUser.RoleId).ToString());

            var company1 = await _helperService.CreateTestCompanyAsync();
            var company2 = await _helperService.CreateTestCompanyAsync();

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
                p.Price == product2.Price
            );

            result.Items.Should().ContainSingle(p =>
                p.Id == product3.Id &&
                p.RegistrationNumber == product3.RegistrationNumber &&
                p.Name == product3.Name &&
                p.CompanyId == product3.CompanyId &&
                p.Price == product3.Price
            );

            //Clean up
            context.Product.Remove(product1);
            context.Product.Remove(product2);
            context.Product.Remove(product3);
            await context.SaveChangesAsync();

            context.User.Remove(testUser);
            context.Company.Remove(company1);
            context.Company.Remove(company2);
            await context.SaveChangesAsync();
        }*/
    }
}