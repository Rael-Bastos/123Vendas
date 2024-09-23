using _123Vendas.Vendas.Data.Context;
using _123Vendas.Vendas.Data.Entity;
using _123Vendas.Vendas.Data.Repository;
using Bogus;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace _123Vendas.Vendas.Data.Tests.Repository
{
    public class SalesRepositoryTests
    {
        private readonly SalesDbContext _dbContext;
        private readonly ILogger<SalesRepository> _logger;
        private readonly SalesRepository _repository;
        private readonly Faker<Sale> _saleFaker;
        private readonly Faker<SaleItem> _saleItemFaker;

        public SalesRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<SalesDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _dbContext = new SalesDbContext(options);
            _logger = Substitute.For<ILogger<SalesRepository>>();
            _repository = new SalesRepository(_dbContext, _logger);

            // Inicializando Bogus para gerar dados fictícios
            _saleItemFaker = new Faker<SaleItem>()
                .RuleFor(i => i.SaleNumber, f => Guid.NewGuid())
                .RuleFor(i => i.ProductId, f => Guid.NewGuid())
                .RuleFor(i => i.ProductDescription, f => f.Commerce.ProductName())
                .RuleFor(i => i.Quantity, f => f.Random.Int(1, 10))
                .RuleFor(i => i.UnitPrice, f => f.Finance.Amount())
                .RuleFor(i => i.Discount, f => f.Finance.Amount(0, 100))
                .RuleFor(i => i.TotalPrice, (f, i) => (i.UnitPrice - i.Discount) * i.Quantity)
                .RuleFor(i => i.IsCanceled, f => f.Random.Bool());

            _saleFaker = new Faker<Sale>()
                .RuleFor(s => s.SaleNumber, f => Guid.NewGuid())
                .RuleFor(s => s.SaleDate, f => f.Date.Past())
                .RuleFor(s => s.Branch, f => f.Company.CompanyName())
                .RuleFor(s => s.CustomerId, f => Guid.NewGuid().ToString())
                .RuleFor(s => s.TotalAmount, f => f.Finance.Amount())
                .RuleFor(s => s.Items, f => _saleItemFaker.Generate(3));
        }

        [Fact]
        public async Task GetSaleByIdAsync_ShouldReturnSale_WhenSaleExists()
        {
            // Arrange
            var sale = _saleFaker.Generate();
            await _dbContext.Sales.AddAsync(sale);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _repository.GetSaleByIdAsync(sale.SaleNumber);

            // Assert
            result.Should().NotBeNull();
            result.SaleNumber.Should().Be(sale.SaleNumber);
        }

        [Fact]
        public async Task GetSaleByIdAsync_ShouldLogWarning_WhenSaleDoesNotExist()
        {
            // Arrange
            var saleNumber = Guid.NewGuid();

            // Act
            var result = await _repository.GetSaleByIdAsync(saleNumber);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task CreateSaleAsync_ShouldAddSale()
        {
            // Arrange
            var sale = _saleFaker.Generate();

            // Act
            await _repository.CreateSaleAsync(sale);
            var savedSale = await _dbContext.Sales.FindAsync(sale.SaleNumber);

            // Assert
            savedSale.Should().NotBeNull();
            savedSale.SaleNumber.Should().Be(sale.SaleNumber);
        }

        [Fact]
        public async Task UpdateSaleAsync_ShouldUpdateSale()
        {
            // Arrange
            var sale = _saleFaker.Generate();
            await _dbContext.Sales.AddAsync(sale);
            await _dbContext.SaveChangesAsync();

            sale.Branch = "UpdatedBranch";

            // Act
            await _repository.UpdateSaleAsync(sale);
            var updatedSale = await _dbContext.Sales.FindAsync(sale.SaleNumber);

            // Assert
            updatedSale.Branch.Should().Be("UpdatedBranch");
        }

        [Fact]
        public async Task DeleteSaleAsync_ShouldDeleteSale_WhenSaleExists()
        {
            // Arrange
            var sale = _saleFaker.Generate();
            await _dbContext.Sales.AddAsync(sale);
            await _dbContext.SaveChangesAsync();

            // Act
            await _repository.DeleteSaleAsync(sale.SaleNumber);
            var deletedSale = await _dbContext.Sales.FindAsync(sale.SaleNumber);

            // Assert
            deletedSale.Should().BeNull();
        }
    }
}