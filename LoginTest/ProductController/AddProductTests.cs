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

namespace StoreAPI.IntegrationTests.CompanyController
{
    public class AddProductIntegrationTests : IClassFixture<CustomWebApplicationFactory>, IDisposable
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory _factory;
        private readonly HelperService _helperService;
        private readonly string prefix = "AddProduct_";

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

            var testUser = await _helperService.CreateTestUserAsync(prefix);
            var token = tokenService.GenerateToken(testUser.Id, ((RoleEnum)testUser.RoleId).ToString());

            var testCompany = await _helperService.CreateTestCompanyAsync(prefix);

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
        }

        [Theory]
        [InlineData("TV Hisense", 900.99)]
        [InlineData("GTX 1650M", 250.50)]
        [InlineData("Iphone", 1000.00)]
        public async Task AddProduct_WithDifferentValidData_ShouldReturnOk(string name, decimal price)
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
            var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

            var testUser = await _helperService.CreateTestUserAsync(prefix);
            var token = tokenService.GenerateToken(testUser.Id, ((RoleEnum)testUser.RoleId).ToString());

            var testCompany = await _helperService.CreateTestCompanyAsync(prefix);

            var addRequest = new AddProductRequest
            {
                Name = prefix + name,
                CompanyId = testCompany.Id,
                Price = price
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
        }

        [Fact]
        public async Task AddProduct_WithNonExistingCompanyId_ShouldReturnNotFound()
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
            var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

            var testUser = await _helperService.CreateTestUserAsync(prefix);
            var token = tokenService.GenerateToken(testUser.Id, ((RoleEnum)testUser.RoleId).ToString());

            var testCompany = await _helperService.CreateTestCompanyAsync(prefix);

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
        }

        [Fact]
        public async Task AddProduct_WithExistingName_ShouldReturnConflict()
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
            var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

            var testUser = await _helperService.CreateTestUserAsync(prefix);
            var token = tokenService.GenerateToken(testUser.Id, ((RoleEnum)testUser.RoleId).ToString());

            var testCompany = await _helperService.CreateTestCompanyAsync(prefix);

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
        }

        [Fact]
        public async Task AddProduct_WithNameTooShort_ShouldReturnBadRequest()
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
            var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

            var testUser = await _helperService.CreateTestUserAsync(prefix);
            var token = tokenService.GenerateToken(testUser.Id, ((RoleEnum)testUser.RoleId).ToString());

            var testCompany = await _helperService.CreateTestCompanyAsync(prefix);

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
        }

        [Fact]
        public async Task AddProduct_WithNameTooLong_ShouldReturnBadRequest()
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
            var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

            var testUser = await _helperService.CreateTestUserAsync(prefix);
            var token = tokenService.GenerateToken(testUser.Id, ((RoleEnum)testUser.RoleId).ToString());

            var testCompany = await _helperService.CreateTestCompanyAsync(prefix);

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
        }

        [Fact]
        public async Task AddProduct_WithPriceTooSmall_ShouldReturnBadRequest()
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
            var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

            var testUser = await _helperService.CreateTestUserAsync(prefix);
            var token = tokenService.GenerateToken(testUser.Id, ((RoleEnum)testUser.RoleId).ToString());

            var testCompany = await _helperService.CreateTestCompanyAsync(prefix);

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
        }

        [Fact]
        public async Task AddProduct_WithMockedReposAndValidData_ShouldReturnOk()
        {
            var productRepoMock = new Mock<IProductRepository>();
            var productSaleRepoMock = new Mock<IProductSaleRepository>();
            var companyRepoMock = new Mock<ICompanyRepository>();

            var company = new Company 
            { 
                Id = 1, 
                Name = _helperService.CreateRandomText() 
            };

            companyRepoMock.Setup(r => r.GetById(company.Id))
                           .ReturnsAsync(company);

            productRepoMock.Setup(r => r.GetByName(It.IsAny<string>()))
                           .ReturnsAsync((Product)null);

            productRepoMock.Setup(r => r.GetByRegistrationNumber(It.IsAny<string>()))
                           .ReturnsAsync((Product)null);

            var service = new ProductService(productRepoMock.Object, productSaleRepoMock.Object, companyRepoMock.Object);
            var controller = new StoreAPI.Controllers.ProductController(Mock.Of<ILogger<StoreAPI.Controllers.ProductController>>(), service);

            var request = new AddProductRequest
            {
                Name = _helperService.CreateRandomText(),
                CompanyId = company.Id,
                Price = _helperService.CreateRandomNumber()
            };

            var result = await controller.AddProduct(request);

            result.Should().BeOfType<OkResult>();
            productRepoMock.Verify(r => r.Add(It.Is<Product>(p =>
                p.Name == request.Name &&
                p.CompanyId == request.CompanyId &&
                p.Price == request.Price
            )), Times.Once);
        }


        public void Dispose()
        {
            _helperService.CleanUp(prefix);
        }
    }
}