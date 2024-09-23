using _123Vendas.Vendas.Infra.Events;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _123Vendas.Vendas.Domain.Tests.Events
{
    public class SaleUpdatedEventTests
    {
        [Fact]
        public void SaleUpdatedEvent_Should_Set_Correct_Properties()
        {
            // Arrange
            var saleId = Guid.NewGuid();
            var newTotalAmount = 1200m;

            // Act
            var updatedEvent = new SaleUpdatedEvent(saleId, newTotalAmount);

            // Assert
            updatedEvent.SaleId.Should().Be(saleId);
            updatedEvent.NewTotalAmount.Should().Be(newTotalAmount);
            updatedEvent.UpdatedAt.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(1));
        }
    }
}
