using _123Vendas.Vendas.Data.Context;
using _123Vendas.Vendas.Data.Entity;
using Bogus;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _123Vendas.Vendas.Data.Tests.Context
{
    public class SalesDbContextTests
    {
        private SalesDbContext CreateDbContext(DbContextOptions<SalesDbContext> options)
        {
            return new SalesDbContext(options);
        }

        private Faker<Sale> _saleFaker;
        private Faker<SaleItem> _saleItemFaker;

        public SalesDbContextTests()
        {
            // Configuração do Bogus para gerar dados fictícios
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
        public void SalesDbContext_Should_ConfigurePrimaryKeys()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<SalesDbContext>()
                .UseInMemoryDatabase("test_database")
                .Options;

            using var context = CreateDbContext(options);

            // Act
            var model = context.Model;

            // Assert
            var saleEntity = model.FindEntityType(typeof(Sale));
            saleEntity.Should().NotBeNull();
            saleEntity.FindPrimaryKey().Properties.Select(p => p.Name).Should().Contain("SaleNumber");

            var saleItemEntity = model.FindEntityType(typeof(SaleItem));
            saleItemEntity.Should().NotBeNull();
            saleItemEntity.FindPrimaryKey().Properties.Select(p => p.Name).Should().Contain(new[] { "SaleNumber", "ProductId" });
        }

        [Fact]
        public async Task SalesDbContext_Should_SaveSaleWithItems()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<SalesDbContext>()
                .UseInMemoryDatabase("test_database")
                .Options;

            using var context = CreateDbContext(options);
            var sale = _saleFaker.Generate();

            // Act
            context.Sales.Add(sale);
            await context.SaveChangesAsync();

            // Assert
            var savedSale = await context.Sales.Include(s => s.Items).FirstOrDefaultAsync(s => s.SaleNumber == sale.SaleNumber);
            savedSale.Should().NotBeNull();
            savedSale.Items.Should().HaveCount(3);
        }
    }
}

