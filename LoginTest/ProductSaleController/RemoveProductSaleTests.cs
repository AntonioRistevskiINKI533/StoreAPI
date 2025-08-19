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
using System.Linq;
using System;

namespace StoreAPI.IntegrationTests.ProductSaleController
{
    public class RemoveProductSaleIntegrationTests : IClassFixture<CustomWebApplicationFactory>, IDisposable
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory _factory;
        private readonly HelperService _helperService;
        private readonly string prefix = "RemoveProductSale_";

        public RemoveProductSaleIntegrationTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
            _helperService = new HelperService(_factory);
        }

        [Fact]
        public async Task RemoveProductSale_WithValidProductSaleId_ShouldReturnOk()
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
            var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

            var testUser = await _helperService.CreateTestUserAsync(prefix);

            var token = tokenService.GenerateToken(testUser.Id, ((RoleEnum)testUser.RoleId).ToString());

            var testCompany = await _helperService.CreateTestCompanyAsync(prefix);

            var testProduct = await _helperService.CreateTestProductAsync(testCompany.Id);

            var testProductSale = await _helperService.CreateTestProductSaleAsync(testProduct.Id);

            var request = new HttpRequestMessage(HttpMethod.Delete, $"/ProductSale/RemoveProductSale?productSaleId={testProductSale.Id}");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _client.SendAsync(request);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

            (await context.ProductSale.AnyAsync(c => c.Id == testProductSale.Id)).Should().BeFalse();
        }

        [Fact]
        public async Task RemoveProductSale_WithNonExistentProductSaleId_ShouldReturnNotFound()
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
            var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

            var testUser = await _helperService.CreateTestUserAsync(prefix);

            var token = tokenService.GenerateToken(testUser.Id, ((RoleEnum)testUser.RoleId).ToString());

            var testCompany = await _helperService.CreateTestCompanyAsync(prefix);

            var testProduct = await _helperService.CreateTestProductAsync(testCompany.Id);

            var testProductSale = await _helperService.CreateTestProductSaleAsync(testProduct.Id);

            context.ProductSale.Remove(testProductSale);
            await context.SaveChangesAsync();

            var request = new HttpRequestMessage(HttpMethod.Delete, $"/ProductSale/RemoveProductSale?productSaleId={testProductSale.Id}");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _client.SendAsync(request);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);

            var content = await response.Content.ReadAsStringAsync();
            content.Should().Be("Product sale does not exist");
        }

        public void Dispose()
        {
            _helperService.CleanUp(prefix);
        }
    }
}