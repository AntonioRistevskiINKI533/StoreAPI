using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using StoreAPI.Services;
using System.Net.Http;
using StoreAPI.Models.Contexts;
using StoreAPI.IntegrationTests.Shared;
using StoreAPI.Enums;
using Microsoft.EntityFrameworkCore;

namespace StoreAPI.IntegrationTests.ProductController
{
    public class RemoveProductIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory _factory;
        private readonly HelperService _helperService;

        public RemoveProductIntegrationTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
            _helperService = new HelperService(_factory);
        }

        [Fact]
        public async Task RemoveProduct_WithValidProductIdAndDeletableProduct_ShouldReturnOk()
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
            var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

            var testUser = await _helperService.CreateTestUserAsync();

            var token = tokenService.GenerateToken(testUser.Id, ((RoleEnum)testUser.RoleId).ToString());

            var testCompany = await _helperService.CreateTestCompanyAsync();

            var testProduct = await _helperService.CreateTestProductAsync(testCompany.Id);

            var request = new HttpRequestMessage(HttpMethod.Delete, $"/Product/RemoveProduct?productId={testProduct.Id}");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _client.SendAsync(request);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

            (await context.Product.AnyAsync(c => c.Id == testProduct.Id)).Should().BeFalse();

            //Clean up
            context.User.Remove(testUser);
            context.Company.Remove(testCompany);
            await context.SaveChangesAsync();
        }

        [Fact]
        public async Task RemoveProduct_WithNonExistentProductId_ShouldReturnNotFound()
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
            context.Company.Remove(testCompany);
            await context.SaveChangesAsync();

            var request = new HttpRequestMessage(HttpMethod.Delete, $"/Product/RemoveProduct?productId={testProduct.Id}");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _client.SendAsync(request);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);

            var content = await response.Content.ReadAsStringAsync();
            content.Should().Be("Product does not exist");

            //Clean up
            context.User.Remove(testUser);
            await context.SaveChangesAsync();
        }

        [Fact]
        public async Task RemoveProduct_WithExistingProductSale_ShouldReturnConflict()
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
            var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

            var testUser = await _helperService.CreateTestUserAsync();

            var token = tokenService.GenerateToken(testUser.Id, ((RoleEnum)testUser.RoleId).ToString());

            var testCompany = await _helperService.CreateTestCompanyAsync();

            var testProduct = await _helperService.CreateTestProductAsync(testCompany.Id);

            var testProductSale = await _helperService.CreateTestProductSaleAsync(testProduct.Id);

            var request = new HttpRequestMessage(HttpMethod.Delete, $"/Product/RemoveProduct?productId={testProduct.Id}");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _client.SendAsync(request);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Conflict);

            var content = await response.Content.ReadAsStringAsync();
            content.Should().Be("Product has product sales, please delete them first");

            (await context.Product.AnyAsync(c => c.Id == testProduct.Id)).Should().BeTrue();

            //Clean up
            context.ProductSale.Remove(testProductSale);
            await context.SaveChangesAsync();

            context.Product.Remove(testProduct);
            await context.SaveChangesAsync();

            context.Company.Remove(testCompany);
            context.User.Remove(testUser);
            await context.SaveChangesAsync();
        }
    }
}