using _123Vendas.Vendas.Infra.Events;
using FluentAssertions;

namespace _123Vendas.Vendas.Domain.Tests.Events
{
    public class SaleCreatedEventTests
    {
        [Fact]
        public void SaleCreatedEvent_Should_Set_Correct_Properties()
        {
            // Arrange
            var saleId = Guid.NewGuid();
            var totalAmount = 1000m;

            // Act
            var createdEvent = new SaleCreatedEvent(saleId, totalAmount);

            // Assert
            createdEvent.SaleId.Should().Be(saleId);
            createdEvent.TotalAmount.Should().Be(totalAmount);
            createdEvent.CreatedAt.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(1));
        }
    }
}
