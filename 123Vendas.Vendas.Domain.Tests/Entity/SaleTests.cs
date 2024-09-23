using _123Vendas.Vendas.Data.Entity;
using Bogus;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _123Vendas.Vendas.Domain.Tests.Entity
{
    public class SaleTests
    {
        private readonly Faker<SaleItem> _saleItemFaker;
        private readonly Faker<Sale> _saleFaker;

        public SaleTests()
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

            // Faker para Sale
            _saleFaker = new Faker<Sale>()
                .RuleFor(s => s.SaleNumber, f => Guid.NewGuid())
                .RuleFor(s => s.SaleDate, f => f.Date.Past())
                .RuleFor(s => s.CustomerId, f => f.Random.Guid().ToString())
                .RuleFor(s => s.Branch, f => f.Address.City())
                .RuleFor(s => s.Items, f => _saleItemFaker.Generate(3))
                .RuleFor(s => s.IsCanceled, false);
        }

        [Fact]
        public void Sale_Should_Have_Valid_Properties()
        {
            // Arrange
            var sale = _saleFaker.Generate();

            // Assert
            sale.SaleNumber.Should().NotBeEmpty();
            sale.SaleDate.Should().BeBefore(DateTime.Now);
            sale.CustomerId.Should().NotBeNullOrEmpty();
            sale.TotalAmount.Should().BeGreaterThan(0);
            sale.Branch.Should().NotBeNullOrEmpty();
            sale.Items.Should().HaveCount(3);
        }

        [Fact]
        public void Sale_Should_Calculate_TotalAmount_Correctly()
        {
            // Arrange
            var sale = _saleFaker.Generate();
            sale.TotalAmount = sale.Items.Sum(x => x.TotalPrice);
            // Act
            decimal totalCalculated = 0;
            foreach (var item in sale.Items)
            {
                totalCalculated += item.TotalPrice;
            }

            // Assert
            sale.TotalAmount.Should().Be(totalCalculated);
        }
    }
}
