using _123Vendas.Vendas.Infra.Events;
using FluentAssertions;

namespace _123Vendas.Vendas.Domain.Tests.Events
{
    public class ItemCanceledEventTests
    {
        [Fact]
        public void ItemCanceledEvent_Should_Set_Correct_Properties()
        {
            // Arrange
            var saleId = Guid.NewGuid();
            var itemId = Guid.NewGuid();
            var productName = "Test Product";

            // Act
            var canceledEvent = new ItemCanceledEvent(saleId, itemId, productName);

            // Assert
            canceledEvent.SaleId.Should().Be(saleId);
            canceledEvent.ItemId.Should().Be(itemId);
            canceledEvent.ProductName.Should().Be(productName);
            canceledEvent.CanceledAt.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(1));
        }
    }
}