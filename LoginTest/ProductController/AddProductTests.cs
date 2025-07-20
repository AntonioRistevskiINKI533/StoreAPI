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

namespace StoreAPI.IntegrationTests.CompanyController
{
    public class AddProductIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory _factory;
        private readonly HelperService _helperService;

        public AddProductIntegrationTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
            _helperService = new HelperService(_factory);
        }

        [Fact]
        public async Task AddProduct_WithValidData_ShouldReturnOk()
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

            var request = new HttpRequestMessage(HttpMethod.Post, "/Product/AddProduct")
            {
                Content = JsonContent.Create(addRequest)
            };
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _client.SendAsync(request);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

            var addedProduct = await context.Product.FirstOrDefaultAsync(u => u.Name == addRequest.Name);
            addedProduct.Should().NotBeNull();
            addedProduct.Name.Should().Be(addRequest.Name);
            addedProduct.CompanyId.Should().Be(addRequest.CompanyId);
            addedProduct.Price.Should().Be(addRequest.Price);

            //Clean up
            context.Product.Remove(addedProduct);
            context.User.Remove(testUser);
            await context.SaveChangesAsync();

            context.Company.Remove(testCompany);
            await context.SaveChangesAsync();
        }

        [Fact]
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
    }
}