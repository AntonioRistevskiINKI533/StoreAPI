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

namespace StoreAPI.IntegrationTests.ProductSaleController
{
    public class AddProductSaleIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory _factory;
        private readonly HelperService _helperService;

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

            var testUser = await _helperService.CreateTestUserAsync();
            var token = tokenService.GenerateToken(testUser.Id, ((RoleEnum)testUser.RoleId).ToString());

            var testCompany = await _helperService.CreateTestCompanyAsync();

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
                u.PricePerUnit == addRequest.PricePerUnit
                //u.Date.ToString() == addRequest.Date.ToString() TODO fix this
                );

            addedProductSale.Should().NotBeNull();

            //Clean up
            context.ProductSale.Remove(addedProductSale!);
            await context.SaveChangesAsync();

            context.Product.Remove(testProduct);
            await context.SaveChangesAsync();

            context.User.Remove(testUser);
            context.Company.Remove(testCompany);
            await context.SaveChangesAsync();
        }

/*        [Fact]
        public async Task AddProduct_WithNonExistingCompanyId_ShouldReturnNotFound()
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
            var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

            var testUser = await _helperService.CreateTestUserAsync();
            var token = tokenService.GenerateToken(testUser.Id, ((RoleEnum)testUser.RoleId).ToString());

            var testCompany = await _helperService.CreateTestCompanyAsync();

            var addRequest = new AddProductRequest
            {
                Name = _helperService.CreateRandomText(),
                CompanyId = testCompany.Id,
                Price = _helperService.CreateRandomPrice()
            };

            context.Company.Remove(testCompany);
            await context.SaveChangesAsync();

            var request = new HttpRequestMessage(HttpMethod.Post, "/Product/AddProduct")
            {
                Content = JsonContent.Create(addRequest)
            };
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _client.SendAsync(request);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);

            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("Company does not exist");

            //Clean up
            context.User.Remove(testUser);
            await context.SaveChangesAsync();
        }

        [Fact]
        public async Task AddProduct_WithExistingName_ShouldReturnConflict()
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
            var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

            var testUser = await _helperService.CreateTestUserAsync();
            var token = tokenService.GenerateToken(testUser.Id, ((RoleEnum)testUser.RoleId).ToString());

            var testCompany = await _helperService.CreateTestCompanyAsync();

            var existingProduct = await _helperService.CreateTestProductAsync(testCompany.Id);

            var addRequest = new AddProductRequest
            {
                Name = existingProduct.Name,
                CompanyId = testCompany.Id,
                Price = _helperService.CreateRandomPrice()
            };

            var request = new HttpRequestMessage(HttpMethod.Post, "/Product/AddProduct")
            {
                Content = JsonContent.Create(addRequest)
            };
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _client.SendAsync(request);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Conflict);

            var content = await response.Content.ReadAsStringAsync();
            content.Should().Be("Product with same name already exists");

            //Clean up
            context.Product.Remove(existingProduct);
            context.User.Remove(testUser);
            await context.SaveChangesAsync();

            context.Company.Remove(testCompany);
            await context.SaveChangesAsync();
        }

        [Fact]
        public async Task AddProduct_WithNameTooShort_ShouldReturnBadRequest()
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
            var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

            var testUser = await _helperService.CreateTestUserAsync();
            var token = tokenService.GenerateToken(testUser.Id, ((RoleEnum)testUser.RoleId).ToString());

            var testCompany = await _helperService.CreateTestCompanyAsync();

            var addRequest = new AddProductRequest
            {
                Name = "",
                CompanyId = testCompany.Id,
                Price = _helperService.CreateRandomPrice()
            };

            var request = new HttpRequestMessage(HttpMethod.Post, "/Product/AddProduct")
            {
                Content = JsonContent.Create(addRequest)
            };
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _client.SendAsync(request);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);

            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("Name must be between 1 and 500 characters");

            //Clean up
            context.User.Remove(testUser);
            context.Company.Remove(testCompany);
            await context.SaveChangesAsync();
        }

        [Fact]
        public async Task AddProduct_WithNameTooLong_ShouldReturnBadRequest()
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
            var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

            var testUser = await _helperService.CreateTestUserAsync();
            var token = tokenService.GenerateToken(testUser.Id, ((RoleEnum)testUser.RoleId).ToString());

            var testCompany = await _helperService.CreateTestCompanyAsync();

            var addRequest = new AddProductRequest
            {
                Name = new string('A', 501),
                CompanyId = testCompany.Id,
                Price = _helperService.CreateRandomPrice()
            };

            var request = new HttpRequestMessage(HttpMethod.Post, "/Product/AddProduct")
            {
                Content = JsonContent.Create(addRequest)
            };
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _client.SendAsync(request);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);

            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("Name must be between 1 and 500 characters");

            //Clean up
            context.User.Remove(testUser);
            context.Company.Remove(testCompany);
            await context.SaveChangesAsync();
        }

        [Fact]
        public async Task AddProduct_WithPriceTooSmall_ShouldReturnBadRequest()
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
            var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

            var testUser = await _helperService.CreateTestUserAsync();
            var token = tokenService.GenerateToken(testUser.Id, ((RoleEnum)testUser.RoleId).ToString());

            var testCompany = await _helperService.CreateTestCompanyAsync();

            var addRequest = new AddProductRequest
            {
                Name = _helperService.CreateRandomText(),
                CompanyId = testCompany.Id,
                Price = 0
            };

            var request = new HttpRequestMessage(HttpMethod.Post, "/Product/AddProduct")
            {
                Content = JsonContent.Create(addRequest)
            };
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _client.SendAsync(request);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);

            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("Price must be greater than 0");

            //Clean up
            context.User.Remove(testUser);
            context.Company.Remove(testCompany);
            await context.SaveChangesAsync();
        }*/
    }
}