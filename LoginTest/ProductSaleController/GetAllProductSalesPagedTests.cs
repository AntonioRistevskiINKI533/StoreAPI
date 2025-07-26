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
using System;

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

            var request = new HttpRequestMessage(HttpMethod.Get, "/ProductSale/GetAllProductSalesPaged?pageIndex=0&pageSize=20");
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
            context.User.Remove(testUser);
            await context.SaveChangesAsync();

            context.Product.Remove(product);
            await context.SaveChangesAsync();

            context.Company.Remove(company);
            await context.SaveChangesAsync();
        }

        [Fact]
        public async Task GetAllProductSalesPaged_PagingTest_ShouldReturnOkAndPagedResult()
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
            var productSale3 = await _helperService.CreateTestProductSaleAsync(product.Id);
            var productSale4 = await _helperService.CreateTestProductSaleAsync(product.Id);
            var productSale5 = await _helperService.CreateTestProductSaleAsync(product.Id);

            var request = new HttpRequestMessage(HttpMethod.Get, $"/ProductSale/GetAllProductSalesPaged?pageIndex=1&pageSize=2&productId={product.Id}");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _client.SendAsync(request);

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<PagedModel<ProductSaleData>>();
            result.Should().NotBeNull();
            result.Items.Should().NotBeNull();
            result.Items.Count.Should().Be(2);

            result.Items.Should().ContainSingle(p =>
                p.Id == productSale3.Id &&
                p.ProductId == productSale3.ProductId &&
                p.SoldAmount == productSale3.SoldAmount &&
                p.PricePerUnit == productSale3.PricePerUnit &&
                p.Date.ToString() == productSale3.Date.ToString() &&
                p.ProductName == product.Name
            );

            result.Items.Should().ContainSingle(p =>
                p.Id == productSale4.Id &&
                p.ProductId == productSale4.ProductId &&
                p.SoldAmount == productSale4.SoldAmount &&
                p.PricePerUnit == productSale4.PricePerUnit &&
                p.Date.ToString() == productSale4.Date.ToString() &&
                p.ProductName == product.Name
            );

            //Clean up
            context.ProductSale.Remove(productSale1);
            context.ProductSale.Remove(productSale2);
            context.ProductSale.Remove(productSale3);
            context.ProductSale.Remove(productSale4);
            context.ProductSale.Remove(productSale5);
            await context.SaveChangesAsync();

            context.Product.Remove(product);
            await context.SaveChangesAsync();

            context.User.Remove(testUser);
            context.Company.Remove(company);
            await context.SaveChangesAsync();
        }

        [Fact]
        public async Task GetAllProductSalesPaged_WithProductIdFilter_ShouldReturnFilteredResults()
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
            var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

            var testUser = await _helperService.CreateTestUserAsync();

            var token = tokenService.GenerateToken(testUser.Id, ((RoleEnum)testUser.RoleId).ToString());

            var company = await _helperService.CreateTestCompanyAsync();

            var product = await _helperService.CreateTestProductAsync(company.Id);
            var product2 = await _helperService.CreateTestProductAsync(company.Id);

            var productSale1 = await _helperService.CreateTestProductSaleAsync(product.Id);
            var productSale2 = await _helperService.CreateTestProductSaleAsync(product2.Id);
            var productSale3 = await _helperService.CreateTestProductSaleAsync(product2.Id);

            var request = new HttpRequestMessage(HttpMethod.Get, $"/ProductSale/GetAllProductSalesPaged?pageIndex=0&pageSize=10&productId={product2.Id}");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _client.SendAsync(request);

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<PagedModel<ProductSaleData>>();
            result.Should().NotBeNull();
            result.Items.Should().NotBeNull();
            result.Items.Count.Should().Be(2);

            result.Items.Should().ContainSingle(p =>
                p.Id == productSale2.Id &&
                p.ProductId == productSale2.ProductId &&
                p.SoldAmount == productSale2.SoldAmount &&
                p.PricePerUnit == productSale2.PricePerUnit &&
                p.Date.ToString() == productSale2.Date.ToString() &&
                p.ProductName == product2.Name
            );

            result.Items.Should().ContainSingle(p =>
                p.Id == productSale3.Id &&
                p.ProductId == productSale3.ProductId &&
                p.SoldAmount == productSale3.SoldAmount &&
                p.PricePerUnit == productSale3.PricePerUnit &&
                p.Date.ToString() == productSale3.Date.ToString() &&
                p.ProductName == product2.Name
            );

            //Clean up
            context.ProductSale.Remove(productSale1);
            context.ProductSale.Remove(productSale2);
            context.ProductSale.Remove(productSale3);
            await context.SaveChangesAsync();

            context.Product.Remove(product);
            context.Product.Remove(product2);
            await context.SaveChangesAsync();

            context.User.Remove(testUser);
            context.Company.Remove(company);
            await context.SaveChangesAsync();
        }

        [Fact]
        public async Task GetAllProductSalesPaged_WithDateFilter_ShouldReturnFilteredResults()
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
            var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

            var testUser = await _helperService.CreateTestUserAsync();

            var token = tokenService.GenerateToken(testUser.Id, ((RoleEnum)testUser.RoleId).ToString());

            var company = await _helperService.CreateTestCompanyAsync();

            var product = await _helperService.CreateTestProductAsync(company.Id);

            var date1 = new DateTime(2025, 1, 1);
            var date2 = new DateTime(2025, 1, 3);
            var date3 = new DateTime(2025, 1, 4);
            var date4 = new DateTime(2025, 1, 6);

            var productSale1 = await _helperService.CreateTestProductSaleAsync(product.Id, date1);
            var productSale2 = await _helperService.CreateTestProductSaleAsync(product.Id, date2);
            var productSale3 = await _helperService.CreateTestProductSaleAsync(product.Id, date3);
            var productSale4 = await _helperService.CreateTestProductSaleAsync(product.Id, date4);

            var request = new HttpRequestMessage(HttpMethod.Get, $"/ProductSale/GetAllProductSalesPaged?pageIndex=0&pageSize=10&dateFrom=2025-01-02&dateTo=2025-01-05");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _client.SendAsync(request);

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<PagedModel<ProductSaleData>>();
            result.Should().NotBeNull();
            result.Items.Should().NotBeNull();
            result.Items.Count.Should().Be(2);

            result.Items.Should().ContainSingle(p =>
                p.Id == productSale2.Id &&
                p.ProductId == productSale2.ProductId &&
                p.SoldAmount == productSale2.SoldAmount &&
                p.PricePerUnit == productSale2.PricePerUnit &&
                p.Date.ToString() == productSale2.Date.ToString() &&
                p.ProductName == product.Name
            );

            result.Items.Should().ContainSingle(p =>
                p.Id == productSale3.Id &&
                p.ProductId == productSale3.ProductId &&
                p.SoldAmount == productSale3.SoldAmount &&
                p.PricePerUnit == productSale3.PricePerUnit &&
                p.Date.ToString() == productSale3.Date.ToString() &&
                p.ProductName == product.Name
            );

            //Clean up
            context.ProductSale.Remove(productSale1);
            context.ProductSale.Remove(productSale2);
            context.ProductSale.Remove(productSale3);
            context.ProductSale.Remove(productSale4);
            await context.SaveChangesAsync();

            context.Product.Remove(product);
            await context.SaveChangesAsync();

            context.User.Remove(testUser);
            context.Company.Remove(company);
            await context.SaveChangesAsync();
        }
    }
}