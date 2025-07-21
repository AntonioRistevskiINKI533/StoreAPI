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

namespace StoreAPI.IntegrationTests.ProductController
{
    public class UpdateProductIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory _factory;
        private readonly HelperService _helperService;

        public UpdateProductIntegrationTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
            _helperService = new HelperService(_factory);
        }

        [Fact]
        public async Task UpdateProduct_WithValidData_ShouldReturnOk()
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
            var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

            var testUser = await _helperService.CreateTestUserAsync();
            var token = tokenService.GenerateToken(testUser.Id, ((RoleEnum)testUser.RoleId).ToString());

            var testCompany = await _helperService.CreateTestCompanyAsync();

            var testProduct = await _helperService.CreateTestProductAsync(testCompany.Id);

            var testCompany2 = await _helperService.CreateTestCompanyAsync();

            var updateRequest = new UpdateProductRequest
            {
                Id = testProduct.Id,
                Name = _helperService.CreateRandomText(),
                CompanyId = testCompany2.Id,
                Price = _helperService.CreateRandomPrice()
            };

            var request = new HttpRequestMessage(HttpMethod.Put, "/Product/UpdateProduct")
            {
                Content = JsonContent.Create(updateRequest)
            };
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _client.SendAsync(request);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

            var updatedProduct = await context.Product.FirstOrDefaultAsync(u => u.Id == updateRequest.Id);
            updatedProduct.Should().NotBeNull();
            updatedProduct.Name.Should().Be(updateRequest.Name);
            updatedProduct.CompanyId.Should().Be(updateRequest.CompanyId);
            updatedProduct.Price.Should().Be(updateRequest.Price);

            context.Product.Remove(updatedProduct);
            await context.SaveChangesAsync();

            //Clean up
            context.Company.Remove(testCompany);
            context.Company.Remove(testCompany2);
            context.User.Remove(testUser);
            await context.SaveChangesAsync();
        }

        [Fact]
        public async Task UpdateProduct_WithNonExistingProductId_ShouldReturnNotFound()
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
            var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

            var testUser = await _helperService.CreateTestUserAsync();
            var token = tokenService.GenerateToken(testUser.Id, ((RoleEnum)testUser.RoleId).ToString());

            var testCompany = await _helperService.CreateTestCompanyAsync();

            var testProduct = await _helperService.CreateTestProductAsync(testCompany.Id);

            context.Product.Remove(testProduct);
            await context.SaveChangesAsync();

            var testCompany2 = await _helperService.CreateTestCompanyAsync();

            var updateRequest = new UpdateProductRequest
            {
                Id = testProduct.Id,
                Name = _helperService.CreateRandomText(),
                CompanyId = testCompany2.Id,
                Price = _helperService.CreateRandomPrice()
            };

            var request = new HttpRequestMessage(HttpMethod.Put, "/Product/UpdateProduct")
            {
                Content = JsonContent.Create(updateRequest)
            };
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _client.SendAsync(request);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);

            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("Product does not exist");

            //Clean up
            context.Company.Remove(testCompany);
            context.Company.Remove(testCompany2);
            context.User.Remove(testUser);
            await context.SaveChangesAsync();
        }

        [Fact]
        public async Task UpdateProduct_WithExistingName_ShouldReturnConflict()
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
            var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

            var testUser = await _helperService.CreateTestUserAsync();
            var token = tokenService.GenerateToken(testUser.Id, ((RoleEnum)testUser.RoleId).ToString());

            var testCompany = await _helperService.CreateTestCompanyAsync();

            var testProduct = await _helperService.CreateTestProductAsync(testCompany.Id);

            var testProduct2 = await _helperService.CreateTestProductAsync(testCompany.Id);

            var testCompany2 = await _helperService.CreateTestCompanyAsync();

            var updateRequest = new UpdateProductRequest
            {
                Id = testProduct.Id,
                Name = testProduct2.Name,
                CompanyId = testCompany2.Id,
                Price = _helperService.CreateRandomPrice()
            };

            var request = new HttpRequestMessage(HttpMethod.Put, "/Product/UpdateProduct")
            {
                Content = JsonContent.Create(updateRequest)
            };
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _client.SendAsync(request);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Conflict);

            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("Product with same name already exists");

            context.Product.Remove(testProduct);
            context.Product.Remove(testProduct2);
            await context.SaveChangesAsync();

            //Clean up
            context.Company.Remove(testCompany);
            context.Company.Remove(testCompany2);
            context.User.Remove(testUser);
            await context.SaveChangesAsync();
        }

        [Fact]
        public async Task UpdateProduct_WithNameTooShort_ShouldReturnBadRequest()
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
            var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

            var testUser = await _helperService.CreateTestUserAsync();
            var token = tokenService.GenerateToken(testUser.Id, ((RoleEnum)testUser.RoleId).ToString());

            var testCompany = await _helperService.CreateTestCompanyAsync();

            var testProduct = await _helperService.CreateTestProductAsync(testCompany.Id);

            var testCompany2 = await _helperService.CreateTestCompanyAsync();

            var updateRequest = new UpdateProductRequest
            {
                Id = testProduct.Id,
                Name = "",
                CompanyId = testCompany2.Id,
                Price = _helperService.CreateRandomPrice()
            };

            var request = new HttpRequestMessage(HttpMethod.Put, "/Product/UpdateProduct")
            {
                Content = JsonContent.Create(updateRequest)
            };
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _client.SendAsync(request);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);

            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("Name must be between 1 and 500 characters");

            context.Product.Remove(testProduct);
            await context.SaveChangesAsync();

            //Clean up
            context.Company.Remove(testCompany);
            context.Company.Remove(testCompany2);
            context.User.Remove(testUser);
            await context.SaveChangesAsync();
        }

        [Fact]
        public async Task UpdateProduct_WithNameTooLong_ShouldReturnBadRequest()
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
            var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

            var testUser = await _helperService.CreateTestUserAsync();
            var token = tokenService.GenerateToken(testUser.Id, ((RoleEnum)testUser.RoleId).ToString());

            var testCompany = await _helperService.CreateTestCompanyAsync();

            var testProduct = await _helperService.CreateTestProductAsync(testCompany.Id);

            var testCompany2 = await _helperService.CreateTestCompanyAsync();

            var updateRequest = new UpdateProductRequest
            {
                Id = testProduct.Id,
                Name = new string('A', 501),
                CompanyId = testCompany2.Id,
                Price = _helperService.CreateRandomPrice()
            };

            var request = new HttpRequestMessage(HttpMethod.Put, "/Product/UpdateProduct")
            {
                Content = JsonContent.Create(updateRequest)
            };
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _client.SendAsync(request);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);

            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("Name must be between 1 and 500 characters");

            context.Product.Remove(testProduct);
            await context.SaveChangesAsync();

            //Clean up
            context.Company.Remove(testCompany);
            context.Company.Remove(testCompany2);
            context.User.Remove(testUser);
            await context.SaveChangesAsync();
        }

        [Fact]
        public async Task UpdateProduct_WithPriceTooSmall_ShouldReturnBadRequest()
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
            var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

            var testUser = await _helperService.CreateTestUserAsync();
            var token = tokenService.GenerateToken(testUser.Id, ((RoleEnum)testUser.RoleId).ToString());

            var testCompany = await _helperService.CreateTestCompanyAsync();

            var testProduct = await _helperService.CreateTestProductAsync(testCompany.Id);

            var testCompany2 = await _helperService.CreateTestCompanyAsync();

            var updateRequest = new UpdateProductRequest
            {
                Id = testProduct.Id,
                Name = _helperService.CreateRandomText(),
                CompanyId = testCompany2.Id,
                Price = 0
            };

            var request = new HttpRequestMessage(HttpMethod.Put, "/Product/UpdateProduct")
            {
                Content = JsonContent.Create(updateRequest)
            };
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _client.SendAsync(request);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);

            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("Price must be greater than 0");

            context.Product.Remove(testProduct);
            await context.SaveChangesAsync();

            //Clean up
            context.Company.Remove(testCompany);
            context.Company.Remove(testCompany2);
            context.User.Remove(testUser);
            await context.SaveChangesAsync();
        }
    }
}