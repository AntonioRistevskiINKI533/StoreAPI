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
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Microsoft.Extensions.Logging;
using StoreAPI.Models;
using StoreAPI.Repositories.Interfaces;

namespace StoreAPI.IntegrationTests.CompanyController
{
    public class AddCompanyIntegrationTests : IClassFixture<CustomWebApplicationFactory>, IDisposable
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory _factory;
        private readonly HelperService _helperService;
        private readonly string prefix = "AddCompany_";

        public AddCompanyIntegrationTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
            _helperService = new HelperService(_factory);
        }

        [Fact]
        public async Task AddCompany_WithValidData_ShouldReturnOk()
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
            var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

            var testUser = await _helperService.CreateTestUserAsync(prefix);
            var token = tokenService.GenerateToken(testUser.Id, ((RoleEnum)testUser.RoleId).ToString());

            var addRequest = new AddCompanyRequest
            {
                Name = prefix + _helperService.CreateRandomText(),
                Address = _helperService.CreateRandomText(),
                Phone = _helperService.CreateRandomPhoneNumber()
            };

            var request = new HttpRequestMessage(HttpMethod.Post, "/Company/AddCompany")
            {
                Content = JsonContent.Create(addRequest)
            };
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _client.SendAsync(request);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

            var addedCompany = await context.Company.FirstOrDefaultAsync(u => u.Name == addRequest.Name);
            addedCompany.Should().NotBeNull();
            addedCompany.Name.Should().Be(addRequest.Name);
            addedCompany.Address.Should().Be(addRequest.Address);
            addedCompany.Phone.Should().Be(addRequest.Phone);
        }

        [Theory]
        [InlineData("Apple", "123 Test Street", "1234567890")]
        [InlineData("Anhoch", "456 test Ave", "0987654321")]
        [InlineData("ACER", "789 Some Blvd", "5555555555")]
        public async Task AddCompany_WithDifferentValidData_ShouldReturnOk(
            string name,
            string address,
            string phone
        )
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
            var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

            var testUser = await _helperService.CreateTestUserAsync(prefix);
            var token = tokenService.GenerateToken(testUser.Id, ((RoleEnum)testUser.RoleId).ToString());

            var addRequest = new AddCompanyRequest
            {
                Name = prefix + name,
                Address = address,
                Phone = phone
            };

            var request = new HttpRequestMessage(HttpMethod.Post, "/Company/AddCompany")
            {
                Content = JsonContent.Create(addRequest)
            };
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _client.SendAsync(request);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

            var addedCompany = await context.Company.FirstOrDefaultAsync(u => u.Name == addRequest.Name);
            addedCompany.Should().NotBeNull();
            addedCompany.Name.Should().Be(addRequest.Name);
            addedCompany.Address.Should().Be(addRequest.Address);
            addedCompany.Phone.Should().Be(addRequest.Phone);
        }

        [Fact]
        public async Task AddCompany_WithExistingName_ShouldReturnConflict()
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
            var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

            var testUser = await _helperService.CreateTestUserAsync(prefix);
            var token = tokenService.GenerateToken(testUser.Id, ((RoleEnum)testUser.RoleId).ToString());

            var testCompany = await _helperService.CreateTestCompanyAsync(prefix);

            var addRequest = new AddCompanyRequest
            {
                Name = testCompany.Name,
                Address = _helperService.CreateRandomText(),
                Phone = _helperService.CreateRandomPhoneNumber()
            };

            var request = new HttpRequestMessage(HttpMethod.Post, "/Company/AddCompany")
            {
                Content = JsonContent.Create(addRequest)
            };
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _client.SendAsync(request);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Conflict);

            var content = await response.Content.ReadAsStringAsync();
            content.Should().Be("Company with same name already exists");
        }

        [Fact]
        public async Task AddCompany_WithExistingAddress_ShouldReturnConflict()
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
            var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

            var testUser = await _helperService.CreateTestUserAsync(prefix);
            var token = tokenService.GenerateToken(testUser.Id, ((RoleEnum)testUser.RoleId).ToString());

            var testCompany = await _helperService.CreateTestCompanyAsync(prefix);

            var addRequest = new AddCompanyRequest
            {
                Name = prefix + _helperService.CreateRandomText(),
                Address = testCompany.Address,
                Phone = _helperService.CreateRandomPhoneNumber()
            };

            var request = new HttpRequestMessage(HttpMethod.Post, "/Company/AddCompany")
            {
                Content = JsonContent.Create(addRequest)
            };
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _client.SendAsync(request);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Conflict);

            var content = await response.Content.ReadAsStringAsync();
            content.Should().Be("Company with same address already exists");
        }

        [Fact]
        public async Task AddCompany_WithExistingPhone_ShouldReturnConflict()
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
            var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

            var testUser = await _helperService.CreateTestUserAsync(prefix);
            var token = tokenService.GenerateToken(testUser.Id, ((RoleEnum)testUser.RoleId).ToString());

            var testCompany = await _helperService.CreateTestCompanyAsync(prefix);

            var addRequest = new AddCompanyRequest
            {
                Name = prefix + _helperService.CreateRandomText(),
                Address = _helperService.CreateRandomText(),
                Phone = testCompany.Phone
            };

            var request = new HttpRequestMessage(HttpMethod.Post, "/Company/AddCompany")
            {
                Content = JsonContent.Create(addRequest)
            };
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _client.SendAsync(request);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Conflict);

            var content = await response.Content.ReadAsStringAsync();
            content.Should().Be("Company with same phone already exists");
        }

        [Fact]
        public async Task AddUser_WithInvalidPhoneFormat_ShouldReturnBadRequest()
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
            var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

            var testUser = await _helperService.CreateTestUserAsync(prefix);
            var token = tokenService.GenerateToken(testUser.Id, ((RoleEnum)testUser.RoleId).ToString());

            var addRequest = new AddCompanyRequest
            {
                Name = prefix + _helperService.CreateRandomText(),
                Address = _helperService.CreateRandomText(),
                Phone = "abc"
            };

            var request = new HttpRequestMessage(HttpMethod.Post, "/Company/AddCompany")
            {
                Content = JsonContent.Create(addRequest)
            };
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _client.SendAsync(request);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);

            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("Invalid phone");
        }

        [Fact]
        public async Task AddUser_WithNameTooShort_ShouldReturnBadRequest()
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
            var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

            var testUser = await _helperService.CreateTestUserAsync(prefix);
            var token = tokenService.GenerateToken(testUser.Id, ((RoleEnum)testUser.RoleId).ToString());

            var addRequest = new AddCompanyRequest
            {
                Name = "",
                Address = _helperService.CreateRandomText(),
                Phone = _helperService.CreateRandomPhoneNumber()
            };

            var request = new HttpRequestMessage(HttpMethod.Post, "/Company/AddCompany")
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
        public async Task AddUser_WithNameTooLong_ShouldReturnBadRequest()
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
            var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

            var testUser = await _helperService.CreateTestUserAsync(prefix);
            var token = tokenService.GenerateToken(testUser.Id, ((RoleEnum)testUser.RoleId).ToString());

            var addRequest = new AddCompanyRequest
            {
                Name = new string('A', 501),
                Address = _helperService.CreateRandomText(),
                Phone = _helperService.CreateRandomPhoneNumber()
            };

            var request = new HttpRequestMessage(HttpMethod.Post, "/Company/AddCompany")
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
        public async Task AddUser_WithAddressTooShort_ShouldReturnBadRequest()
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
            var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

            var testUser = await _helperService.CreateTestUserAsync(prefix);
            var token = tokenService.GenerateToken(testUser.Id, ((RoleEnum)testUser.RoleId).ToString());

            var addRequest = new AddCompanyRequest
            {
                Name = prefix + _helperService.CreateRandomText(),
                Address = "abcd",
                Phone = _helperService.CreateRandomPhoneNumber()
            };

            var request = new HttpRequestMessage(HttpMethod.Post, "/Company/AddCompany")
            {
                Content = JsonContent.Create(addRequest)
            };
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _client.SendAsync(request);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);

            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("Address must be between 5 and 200 characters");
        }

        [Fact]
        public async Task AddUser_WithAddressTooLong_ShouldReturnBadRequest()
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
            var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

            var testUser = await _helperService.CreateTestUserAsync(prefix);
            var token = tokenService.GenerateToken(testUser.Id, ((RoleEnum)testUser.RoleId).ToString());

            var addRequest = new AddCompanyRequest
            {
                Name = prefix + _helperService.CreateRandomText(),
                Address = new string('A', 201),
                Phone = _helperService.CreateRandomPhoneNumber()
            };

            var request = new HttpRequestMessage(HttpMethod.Post, "/Company/AddCompany")
            {
                Content = JsonContent.Create(addRequest)
            };
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _client.SendAsync(request);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);

            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("Address must be between 5 and 200 characters"); ;
        }

        public void Dispose()
        {
            _helperService.CleanUp(prefix);
        }

        [Fact]
        public async Task AddCompany_WithMockedRepoAndWithValidData_ShouldReturnOk()
        {
            var repoMock = new Mock<ICompanyRepository>();
            repoMock.Setup(r => r.GetByNameAddressOrPhone(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), null))
                    .ReturnsAsync((Company)null);

            var productRepoMock = new Mock<IProductRepository>();
            var service = new CompanyService(repoMock.Object, productRepoMock.Object);

            var controller = new StoreAPI.Controllers.CompanyController(Mock.Of<ILogger<StoreAPI.Controllers.CompanyController>>(), service);

            var request = new AddCompanyRequest 
            { 
                Name = _helperService.CreateRandomText(), 
                Address = _helperService.CreateRandomText(), 
                Phone = _helperService.CreateRandomPhoneNumber()
            };

            var result = await controller.AddCompany(request);

            result.Should().BeOfType<OkResult>();
            repoMock.Verify(r => r.Add(It.Is<Company>(c => c.Name == request.Name)), Times.Once);
        }
    }
}