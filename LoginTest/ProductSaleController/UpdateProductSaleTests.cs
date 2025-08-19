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
using System;
using System.Linq;

namespace StoreAPI.IntegrationTests.ProductSaleController
{
    public class UpdateProductSaleIntegrationTests : IClassFixture<CustomWebApplicationFactory>, IDisposable
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory _factory;
        private readonly HelperService _helperService;
        private readonly string prefix = "UpdateProductSale_";

        public UpdateProductSaleIntegrationTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
            _helperService = new HelperService(_factory);
        }

        [Fact]
        public async Task UpdateProductSale_WithValidData_ShouldReturnOk()
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
            var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

            var testUser = await _helperService.CreateTestUserAsync(prefix);
            var token = tokenService.GenerateToken(testUser.Id, ((RoleEnum)testUser.RoleId).ToString());

            var testCompany = await _helperService.CreateTestCompanyAsync(prefix);

            var testProduct = await _helperService.CreateTestProductAsync(testCompany.Id);

            var testProductSale = await _helperService.CreateTestProductSaleAsync(testProduct.Id);

            var testProduct2 = await _helperService.CreateTestProductAsync(testCompany.Id);

            var updateRequest = new UpdateProductSaleRequest
            {
                Id = testProductSale.Id,
                ProductId = testProduct2.Id,
                SoldAmount = _helperService.CreateRandomNumber(),
                PricePerUnit = _helperService.CreateRandomPrice(),
                Date = new DateTime(2025, 5, 10)
            };

            var request = new HttpRequestMessage(HttpMethod.Put, "/ProductSale/UpdateProductSale")
            {
                Content = JsonContent.Create(updateRequest)
            };
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _client.SendAsync(request);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

            var updatedProductSale = await context.ProductSale.FirstOrDefaultAsync(u => u.Id == updateRequest.Id);
            updatedProductSale.Should().NotBeNull();
            updatedProductSale.ProductId.Should().Be(updateRequest.ProductId);
            updatedProductSale.SoldAmount.Should().Be(updateRequest.SoldAmount);
            updatedProductSale.PricePerUnit.Should().Be(updateRequest.PricePerUnit);
            updatedProductSale.Date.Should().Be(updateRequest.Date);
        }

        [Fact]
        public async Task UpdateProductSale__WithNonExistentProductSaleId_ShouldReturnNotFound()
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
            var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

            var testUser = await _helperService.CreateTestUserAsync(prefix);
            var token = tokenService.GenerateToken(testUser.Id, ((RoleEnum)testUser.RoleId).ToString());

            var testCompany = await _helperService.CreateTestCompanyAsync(prefix);

            var testProduct = await _helperService.CreateTestProductAsync(testCompany.Id);

            var testProductSale = await _helperService.CreateTestProductSaleAsync(testProduct.Id);

            var testProduct2 = await _helperService.CreateTestProductAsync(testCompany.Id);

            context.ProductSale.Remove(testProductSale);
            await context.SaveChangesAsync();

            var updateRequest = new UpdateProductSaleRequest
            {
                Id = testProductSale.Id,
                ProductId = testProduct2.Id,
                SoldAmount = _helperService.CreateRandomNumber(),
                PricePerUnit = _helperService.CreateRandomPrice(),
                Date = new DateTime(2025, 5, 10)
            };

            var request = new HttpRequestMessage(HttpMethod.Put, "/ProductSale/UpdateProductSale")
            {
                Content = JsonContent.Create(updateRequest)
            };
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _client.SendAsync(request);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);

            var content = await response.Content.ReadAsStringAsync();
            content.Should().Be("Product sale does not exist");
        }

        [Fact]
        public async Task UpdateProductSale_WithNonExistingProductId_ShouldReturnNotFound()
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
            var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

            var testUser = await _helperService.CreateTestUserAsync(prefix);
            var token = tokenService.GenerateToken(testUser.Id, ((RoleEnum)testUser.RoleId).ToString());

            var testCompany = await _helperService.CreateTestCompanyAsync(prefix);

            var testProduct = await _helperService.CreateTestProductAsync(testCompany.Id);

            var testProductSale = await _helperService.CreateTestProductSaleAsync(testProduct.Id);

            var testProduct2 = await _helperService.CreateTestProductAsync(testCompany.Id);

            context.Product.Remove(testProduct2);
            await context.SaveChangesAsync();

            var updateRequest = new UpdateProductSaleRequest
            {
                Id = testProductSale.Id,
                ProductId = testProduct2.Id,
                SoldAmount = _helperService.CreateRandomNumber(),
                PricePerUnit = _helperService.CreateRandomPrice(),
                Date = new DateTime(2025, 5, 10)
            };

            var request = new HttpRequestMessage(HttpMethod.Put, "/ProductSale/UpdateProductSale")
            {
                Content = JsonContent.Create(updateRequest)
            };
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _client.SendAsync(request);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);

            var content = await response.Content.ReadAsStringAsync();
            content.Should().Be("Product does not exist");
        }

        [Fact]
        public async Task UpdateProductSale_WithSoldAmountTooSmall_ShouldReturnBadRequest()
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
            var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

            var testUser = await _helperService.CreateTestUserAsync(prefix);
            var token = tokenService.GenerateToken(testUser.Id, ((RoleEnum)testUser.RoleId).ToString());

            var testCompany = await _helperService.CreateTestCompanyAsync(prefix);

            var testProduct = await _helperService.CreateTestProductAsync(testCompany.Id);

            var testProductSale = await _helperService.CreateTestProductSaleAsync(testProduct.Id);

            var testProduct2 = await _helperService.CreateTestProductAsync(testCompany.Id);

            var updateRequest = new UpdateProductSaleRequest
            {
                Id = testProductSale.Id,
                ProductId = testProduct2.Id,
                SoldAmount = 0,
                PricePerUnit = _helperService.CreateRandomPrice(),
                Date = new DateTime(2025, 5, 10)
            };

            var request = new HttpRequestMessage(HttpMethod.Put, "/ProductSale/UpdateProductSale")
            {
                Content = JsonContent.Create(updateRequest)
            };
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _client.SendAsync(request);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);

            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("Sold amount must be greater than 0");
        }

        [Fact]
        public async Task UpdateProductSale_WithPriceTooSmall_ShouldReturnBadRequest()
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
            var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

            var testUser = await _helperService.CreateTestUserAsync(prefix);
            var token = tokenService.GenerateToken(testUser.Id, ((RoleEnum)testUser.RoleId).ToString());

            var testCompany = await _helperService.CreateTestCompanyAsync(prefix);

            var testProduct = await _helperService.CreateTestProductAsync(testCompany.Id);

            var testProductSale = await _helperService.CreateTestProductSaleAsync(testProduct.Id);

            var testProduct2 = await _helperService.CreateTestProductAsync(testCompany.Id);

            var updateRequest = new UpdateProductSaleRequest
            {
                Id = testProductSale.Id,
                ProductId = testProduct2.Id,
                SoldAmount = _helperService.CreateRandomNumber(),
                PricePerUnit = 0,
                Date = new DateTime(2025, 5, 10)
            };

            var request = new HttpRequestMessage(HttpMethod.Put, "/ProductSale/UpdateProductSale")
            {
                Content = JsonContent.Create(updateRequest)
            };
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _client.SendAsync(request);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);

            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("Price must be greater than 0");
        }

        public void Dispose()
        {
            _helperService.CleanUp(prefix);
        }
    }
}