using _123Vendas.Vendas.Data.Entity;
using Bogus;
using FluentAssertions;

namespace _123Vendas.Vendas.Domain.Tests.Entity
{
    public class SaleItemTests
    {
        private readonly Faker<SaleItem> _saleItemFaker;

        public SaleItemTests()
        {
            // Faker para SaleItem
            _saleItemFaker = new Faker<SaleItem>()
                .RuleFor(i => i.SaleNumber, f => Guid.NewGuid())
                .RuleFor(i => i.ProductId, f => Guid.NewGuid())
                .RuleFor(i => i.ProductDescription, f => f.Commerce.ProductName())
                .RuleFor(i => i.Quantity, f => f.Random.Int(1, 10))
                .RuleFor(i => i.UnitPrice, f => f.Finance.Amount())
                .RuleFor(i => i.Discount, f => f.Finance.Amount(0, 50))
                .RuleFor(i => i.TotalPrice, (f, i) => (i.Quantity * i.UnitPrice) - i.Discount)
                .RuleFor(i => i.IsCanceled, false);
        }

        [Fact]
        public void SaleItem_Should_Have_Valid_Properties()
        {
            // Arrange
            var saleItem = _saleItemFaker.Generate();

            // Assert
            saleItem.SaleNumber.Should().NotBeEmpty();
            saleItem.ProductId.Should().NotBeEmpty();
            saleItem.ProductDescription.Should().NotBeNullOrEmpty();
            saleItem.Quantity.Should().BeGreaterThan(0);
            saleItem.UnitPrice.Should().BeGreaterThan(0);
            saleItem.TotalPrice.Should().Be((saleItem.Quantity * saleItem.UnitPrice) - saleItem.Discount);
        }
    }
}
