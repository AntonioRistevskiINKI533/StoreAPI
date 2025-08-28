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
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using StoreAPI.Models;
using StoreAPI.Repositories.Interfaces;

namespace StoreAPI.IntegrationTests.ProductSaleController
{
    public class AddProductSaleIntegrationTests : IClassFixture<CustomWebApplicationFactory>, IDisposable
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory _factory;
        private readonly HelperService _helperService;
        private readonly string prefix = "AddProductSale_";

        public AddProductSaleIntegrationTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
            _helperService = new HelperService(_factory);
        }

        [Fact]
        public async Task AddProductSale_WithValidData_ShouldReturnOk()
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
            var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

            var testUser = await _helperService.CreateTestUserAsync(prefix);
            var token = tokenService.GenerateToken(testUser.Id, ((RoleEnum)testUser.RoleId).ToString());

            var testCompany = await _helperService.CreateTestCompanyAsync(prefix);

            var testProduct = await _helperService.CreateTestProductAsync(testCompany.Id);

            var addRequest = new AddProductSaleRequest
            {
                ProductId = testProduct.Id,
                SoldAmount = _helperService.CreateRandomNumber(),
                PricePerUnit = _helperService.CreateRandomPrice(),
                Date = new DateTime(2025, 6, 12)
            };

            var request = new HttpRequestMessage(HttpMethod.Post, "/ProductSale/AddProductSale")
            {
                Content = JsonContent.Create(addRequest)
            };
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _client.SendAsync(request);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

            var addedProductSale = await context.ProductSale.FirstOrDefaultAsync(
                u => u.ProductId == addRequest.ProductId &&
                u.SoldAmount == addRequest.SoldAmount &&
                u.PricePerUnit == addRequest.PricePerUnit &&
                u.Date == addRequest.Date
                );

            addedProductSale.Should().NotBeNull();
        }

        [Theory]
        [InlineData(5, 9.99, "2000-06-12")]
        [InlineData(10, 19.50, "2025-07-01")]
        [InlineData(20, 100.00, "2010-08-15")]
        public async Task AddProductSale_WithDifferentValidData_ShouldReturnOk(
            int soldAmount,
            decimal pricePerUnit,
            string dateString
        )
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
            var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

            var testUser = await _helperService.CreateTestUserAsync(prefix);
            var token = tokenService.GenerateToken(testUser.Id, ((RoleEnum)testUser.RoleId).ToString());

            var testCompany = await _helperService.CreateTestCompanyAsync(prefix);
            var testProduct = await _helperService.CreateTestProductAsync(testCompany.Id);

            var date = DateTime.Parse(dateString);

            var addRequest = new AddProductSaleRequest
            {
                ProductId = testProduct.Id,
                SoldAmount = soldAmount,
                PricePerUnit = pricePerUnit,
                Date = date
            };

            var request = new HttpRequestMessage(HttpMethod.Post, "/ProductSale/AddProductSale")
            {
                Content = JsonContent.Create(addRequest)
            };
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _client.SendAsync(request);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

            var addedProductSale = await context.ProductSale.FirstOrDefaultAsync(
                u => u.ProductId == addRequest.ProductId &&
                     u.SoldAmount == addRequest.SoldAmount &&
                     u.PricePerUnit == addRequest.PricePerUnit &&
                     u.Date == addRequest.Date
            );

            addedProductSale.Should().NotBeNull();
        }

        [Fact]
        public async Task AddProductSale_WithValidDataAndMissingPriceAndDate_ShouldReturnOk()
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
            var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

            var testUser = await _helperService.CreateTestUserAsync(prefix);
            var token = tokenService.GenerateToken(testUser.Id, ((RoleEnum)testUser.RoleId).ToString());

            var testCompany = await _helperService.CreateTestCompanyAsync(prefix);

            var testProduct = await _helperService.CreateTestProductAsync(testCompany.Id);

            var addRequest = new AddProductSaleRequest
            {
                ProductId = testProduct.Id,
                SoldAmount = _helperService.CreateRandomNumber()
            };

            var request = new HttpRequestMessage(HttpMethod.Post, "/ProductSale/AddProductSale")
            {
                Content = JsonContent.Create(addRequest)
            };
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var date = DateTime.Now;
            var response = await _client.SendAsync(request);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

            var addedProductSale = await context.ProductSale.FirstOrDefaultAsync(
                u => u.ProductId == addRequest.ProductId &&
                u.SoldAmount == addRequest.SoldAmount &&
                u.PricePerUnit == testProduct.Price && // should take the products price if none sent
                u.Date >= date && u.Date < date.AddSeconds(5) // should take momentary date if none sent (added check for maximum 5 seconds difference since call can take longer and the date.now gets a second plus)
                );

            addedProductSale.Should().NotBeNull();
        }

        [Fact]
        public async Task AddProductSale_WithNonExistingProductId_ShouldReturnNotFound()
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
            var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

            var testUser = await _helperService.CreateTestUserAsync(prefix);
            var token = tokenService.GenerateToken(testUser.Id, ((RoleEnum)testUser.RoleId).ToString());

            var testCompany = await _helperService.CreateTestCompanyAsync(prefix);

            var testProduct = await _helperService.CreateTestProductAsync(testCompany.Id);

            context.Product.Remove(testProduct);
            await context.SaveChangesAsync();

            var addRequest = new AddProductSaleRequest
            {
                ProductId = testProduct.Id,
                SoldAmount = _helperService.CreateRandomNumber(),
                PricePerUnit = _helperService.CreateRandomPrice(),
                Date = new DateTime(2025, 6, 12)
            };

            var request = new HttpRequestMessage(HttpMethod.Post, "/ProductSale/AddProductSale")
            {
                Content = JsonContent.Create(addRequest)
            };
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _client.SendAsync(request);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);

            var content = await response.Content.ReadAsStringAsync();
            content.Should().Be("Product does not exist");
        }

        [Fact]
        public async Task AddProductSale_WithPriceTooSmall_ShouldReturnBadRequest()
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
            var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

            var testUser = await _helperService.CreateTestUserAsync(prefix);
            var token = tokenService.GenerateToken(testUser.Id, ((RoleEnum)testUser.RoleId).ToString());

            var testCompany = await _helperService.CreateTestCompanyAsync(prefix);

            var testProduct = await _helperService.CreateTestProductAsync(testCompany.Id);

            var addRequest = new AddProductSaleRequest
            {
                ProductId = testProduct.Id,
                SoldAmount = _helperService.CreateRandomNumber(),
                PricePerUnit = 0
            };

            var request = new HttpRequestMessage(HttpMethod.Post, "/ProductSale/AddProductSale")
            {
                Content = JsonContent.Create(addRequest)
            };
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _client.SendAsync(request);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);

            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("Price must be greater than 0");
        }

        [Fact]
        public async Task AddProductSale_WithSoldAmountTooSmall_ShouldReturnBadRequest()
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
            var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

            var testUser = await _helperService.CreateTestUserAsync(prefix);
            var token = tokenService.GenerateToken(testUser.Id, ((RoleEnum)testUser.RoleId).ToString());

            var testCompany = await _helperService.CreateTestCompanyAsync(prefix);

            var testProduct = await _helperService.CreateTestProductAsync(testCompany.Id);

            var addRequest = new AddProductSaleRequest
            {
                ProductId = testProduct.Id,
                SoldAmount = 0,
                PricePerUnit = _helperService.CreateRandomPrice()
            };

            var request = new HttpRequestMessage(HttpMethod.Post, "/ProductSale/AddProductSale")
            {
                Content = JsonContent.Create(addRequest)
            };
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _client.SendAsync(request);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);

            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("Sold amount must be greater than 0");
        }

        [Fact]
        public async Task AddProductSale_WithMockedReposAndValidData_ShouldReturnOk()
        {
            var productRepoMock = new Mock<IProductRepository>();
            var productSaleRepoMock = new Mock<IProductSaleRepository>();

            var product = new Product
            {
                Id = 1,
                Name = _helperService.CreateRandomText(),
                CompanyId = 1,
                Price = _helperService.CreateRandomPrice()
            };

            productRepoMock.Setup(r => r.GetById(product.Id))
                           .ReturnsAsync(product);

            var service = new ProductSaleService(productSaleRepoMock.Object, productRepoMock.Object);
            var controller = new StoreAPI.Controllers.ProductSaleController(Mock.Of<ILogger<StoreAPI.Controllers.ProductSaleController>>(), service);

            var request = new AddProductSaleRequest
            {
                ProductId = product.Id,
                SoldAmount = _helperService.CreateRandomNumber(),
                PricePerUnit = null, //should fall back to product.Price
                Date = DateTime.UtcNow
            };

            var result = await controller.AddProductSale(request);

            result.Should().BeOfType<OkResult>();
            productSaleRepoMock.Verify(r => r.Add(It.Is<ProductSale>(ps =>
                ps.ProductId == request.ProductId &&
                ps.SoldAmount == request.SoldAmount &&
                ps.PricePerUnit == product.Price //since PricePerUnit was null
            )), Times.Once);
        }

        public void Dispose()
        {
            _helperService.CleanUp(prefix);
        }
    }
}