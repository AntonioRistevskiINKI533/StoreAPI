using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using StoreAPI.Services;
using System.Net.Http;
using StoreAPI.Models.Contexts;
using StoreAPI.Models.Datas;
using StoreAPI.IntegrationTests.Shared;
using StoreAPI.Enums;
using System.Linq;
using System;

namespace StoreAPI.IntegrationTests.CompanyController
{
    public class GetProductIntegrationTests : IClassFixture<CustomWebApplicationFactory>, IDisposable
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory _factory;
        private readonly HelperService _helperService;
        private readonly string prefix = "GetProduct_";

        public GetProductIntegrationTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
            _helperService = new HelperService(_factory);
        }

        [Fact]
        public async Task GetProduct_WithValidProductIdAndToken_ShouldReturnOk()
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
            var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

            var testUser = await _helperService.CreateTestUserAsync(prefix);

            var token = tokenService.GenerateToken(testUser.Id, ((RoleEnum)testUser.RoleId).ToString());

            var testCompany = await _helperService.CreateTestCompanyAsync(prefix);

            var testProduct = await _helperService.CreateTestProductAsync(testCompany.Id);

            var request = new HttpRequestMessage(HttpMethod.Get, $"/Product/GetProduct?productId={testProduct.Id}");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _client.SendAsync(request);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

            var productData = await response.Content.ReadFromJsonAsync<ProductData>();
            productData.Should().NotBeNull();
            productData.Id.Should().Be(testProduct.Id);
            productData.RegistrationNumber.Should().Be(testProduct.RegistrationNumber);
            productData.Name.Should().Be(testProduct.Name);
            productData.CompanyId.Should().Be(testProduct.CompanyId);
            productData.Price.Should().Be(testProduct.Price);
        }

        [Fact]
        public async Task GetProduct_WithNonExistentProductId_ShouldReturnNotFound()
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

            var request = new HttpRequestMessage(HttpMethod.Get, $"/Product/GetProduct?productId={testProduct.Id}");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _client.SendAsync(request);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);

            var content = await response.Content.ReadAsStringAsync();
            content.Should().Be("Product does not exist");
        }

        public void Dispose()
        {
            _helperService.CleanUp(prefix);
        }
    }
}