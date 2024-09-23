using _123Vendas.Vendas.Data.Entity;
using Bogus;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Net.Http.Json;
using System.Net;
using Microsoft.AspNetCore.Hosting;

namespace _123Vendas.Vendas.API.Tests.Controllers
{
    public class SalesControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly Faker<Sale> _saleFaker;
        private readonly Faker<SaleItem> _saleItemFaker;

        public SalesControllerIntegrationTests(CustomWebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
            _saleItemFaker = new Faker<SaleItem>()
        .RuleFor(i => i.SaleNumber, f => Guid.NewGuid())
        .RuleFor(i => i.ProductId, f => Guid.NewGuid())
        .RuleFor(i => i.ProductDescription, f => f.Commerce.ProductName())
        .RuleFor(i => i.Quantity, f => f.Random.Int(1, 100))
        .RuleFor(i => i.UnitPrice, f => f.Finance.Amount(1, 1000))
        .RuleFor(i => i.Discount, f => f.Finance.Amount(0, 100))
        .RuleFor(i => i.TotalPrice, (f, i) => (i.UnitPrice * i.Quantity) - i.Discount)
        .RuleFor(i => i.IsCanceled, f => f.Random.Bool());

            _saleFaker = new Faker<Sale>()
                .RuleFor(s => s.SaleNumber, f => Guid.NewGuid())
                .RuleFor(s => s.SaleDate, f => f.Date.Future())
                .RuleFor(s => s.Branch, f => f.Company.CompanyName())
        .RuleFor(s => s.CustomerId, f => Guid.NewGuid().ToString())
        .RuleFor(s => s.TotalAmount, f => f.Finance.Amount(0, 100))
        .RuleFor(s => s.Items, f => _saleItemFaker.Generate(3));
        }

        [Fact]
        public async Task GetSale_ReturnsOk_WhenSaleExists()
        {
            // Arrange
            var sale = _saleFaker.Generate();
            var content = JsonContent.Create(sale);
            await _client.PostAsync("/api/sales", content);

            // Act
            var response = await _client.GetAsync($"/api/sales/{sale.SaleNumber}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var retrievedSale = await response.Content.ReadFromJsonAsync<Sale>();
            retrievedSale.Should().NotBeNull();
            retrievedSale.SaleNumber.Should().Be(sale.SaleNumber);
        }

        [Fact]
        public async Task GetSale_ReturnsNotFound_WhenSaleDoesNotExist()
        {
            // Arrange
            var saleNumber = Guid.NewGuid();

            // Act
            var response = await _client.GetAsync($"/api/sales/{saleNumber}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task CreateSale_ReturnsCreatedAtAction_WhenSaleIsCreated()
        {
            // Arrange
            var sale = _saleFaker.Generate();

            // Act
            var response = await _client.PostAsJsonAsync("/api/sales", sale);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var createdResult = await response.Content.ReadFromJsonAsync<Sale>();
            createdResult.SaleNumber.Should().Be(sale.SaleNumber);
        }

        [Fact]
        public async Task UpdateSale_ReturnsNoContent_WhenSaleIsUpdated()
        {
            // Arrange
            var sale = _saleFaker.Generate();
            await _client.PostAsJsonAsync("/api/sales", sale);
            var updatedSale = _saleFaker.Generate();
            updatedSale.SaleNumber = sale.SaleNumber;
            // Act
            var response = await _client.PutAsJsonAsync($"/api/sales/{sale.SaleNumber}", updatedSale);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task UpdateSale_ReturnsBadRequest_WhenSaleNumberDoesNotMatch()
        {
            // Arrange
            var sale = _saleFaker.Generate();
            await _client.PostAsJsonAsync("/api/sales", sale);
            var updatedSale = new Sale { SaleNumber = Guid.NewGuid(), TotalAmount = 200 };

            // Act
            var response = await _client.PutAsJsonAsync($"/api/sales/{sale.SaleNumber}", updatedSale);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task DeleteSale_ReturnsNoContent_WhenSaleIsDeleted()
        {
            // Arrange
            var sale = _saleFaker.Generate();
            await _client.PostAsJsonAsync("/api/sales", sale);

            // Act
            var response = await _client.DeleteAsync($"/api/sales/{sale.SaleNumber}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task CancelSale_ReturnsOk_WhenSaleIsCanceled()
        {
            // Arrange
            var sale = _saleFaker.Generate();
            var content = JsonContent.Create(sale);
            await _client.PostAsync("/api/sales", content);

            // Act
            var response = await _client.PostAsync($"/api/sales/{sale.SaleNumber}/cancel", null);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var okResult = await response.Content.ReadAsStringAsync();
            ((string)okResult).Should().Be($"Venda {sale.SaleNumber} cancelada com sucesso");
        }

        [Fact]
        public async Task CancelSale_ReturnsNotFound_WhenSaleDoesNotExist()
        {
            // Arrange
            var saleNumber = Guid.NewGuid();

            // Act
            var response = await _client.PostAsync($"/api/sales/{saleNumber}/cancel", null);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task CancelSaleItem_ReturnsOk_WhenItemIsCanceled()
        {
            // Arrange
            var sale = _saleFaker.Generate();
            await _client.PostAsJsonAsync("/api/sales", sale);
            var productId = sale.Items.First().ProductId;

            // Act
            var response = await _client.PostAsync($"/api/sales/{sale.SaleNumber}/items/{productId}/cancel", null);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var okResult = await response.Content.ReadAsStringAsync();
            ((string)okResult).Should().Be($"Item {productId} da venda {sale.SaleNumber} cancelado com sucesso");
        }

        [Fact]
        public async Task CancelSaleItem_ReturnsNotFound_WhenSaleDoesNotExist()
        {
            // Arrange
            var saleNumber = Guid.NewGuid();
            var productId = Guid.NewGuid();

            // Act
            var response = await _client.PostAsync($"/api/sales/{saleNumber}/items/{productId}/cancel", null);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
