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

namespace StoreAPI.IntegrationTests.ProductController
{
    public class GetAllProductsPagedIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory _factory;
        private readonly HelperService _helperService;

        public GetAllProductsPagedIntegrationTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
            _helperService = new HelperService(_factory);
        }

        [Fact]
        public async Task GetAllProductsPaged_WithValidToken_ShouldReturnOkAndPagedResult()
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
            var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

            var testUser = await _helperService.CreateTestUserAsync();

            var token = tokenService.GenerateToken(testUser.Id, ((RoleEnum)testUser.RoleId).ToString());

            var company = await _helperService.CreateTestCompanyAsync();

            var product1 = await _helperService.CreateTestProductAsync(company.Id);
            var product2 = await _helperService.CreateTestProductAsync(company.Id);

            var request = new HttpRequestMessage(HttpMethod.Get, "/Product/GetAllProductsPaged?pageIndex=0&pageSize=10");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _client.SendAsync(request);

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<PagedModel<ProductData>>();
            result.Should().NotBeNull();
            result.Items.Should().NotBeNull();
            result.Items.Count.Should().BeGreaterThanOrEqualTo(2); //should contain at least the two created products

            result.Items.Should().ContainSingle(p =>
                p.Id == product1.Id &&
                p.RegistrationNumber == product1.RegistrationNumber &&
                p.Name == product1.Name &&
                p.CompanyId == product1.CompanyId &&
                p.Price == product1.Price
            );

            result.Items.Should().ContainSingle(p =>
                p.Id == product2.Id &&
                p.RegistrationNumber == product2.RegistrationNumber &&
                p.Name == product2.Name &&
                p.CompanyId == product2.CompanyId &&
                p.Price == product2.Price
            );

            //Clean up
            context.Product.Remove(product1);
            context.Product.Remove(product2);
            await context.SaveChangesAsync();

            context.User.Remove(testUser);
            context.Company.Remove(company);
            await context.SaveChangesAsync();
        }
    }
}