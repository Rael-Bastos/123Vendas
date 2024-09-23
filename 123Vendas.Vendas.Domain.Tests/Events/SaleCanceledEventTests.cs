using _123Vendas.Vendas.Infra.Events;
using FluentAssertions;

namespace _123Vendas.Vendas.Domain.Tests.Events
{
    public class SaleCanceledEventTests
    {
        [Fact]
        public void SaleCanceledEvent_Should_Set_Correct_Properties()
        {
            // Arrange
            var saleId = Guid.NewGuid();

            // Act
            var canceledEvent = new SaleCanceledEvent(saleId);

            // Assert
            canceledEvent.SaleId.Should().Be(saleId);
            canceledEvent.CanceledAt.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(1));
        }
    }
}
