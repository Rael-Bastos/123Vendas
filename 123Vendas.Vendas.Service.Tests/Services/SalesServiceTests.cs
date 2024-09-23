using _123Vendas.Vendas.Data.Entity;
using _123Vendas.Vendas.Infra.Events;
using _123Vendas.Vendas.Infra.Interface.Events;
using _123Vendas.Vendas.Infra.Interface.Repository;
using _123Vendas.Vendas.Service.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace _123Vendas.Vendas.Service.Tests.Services
{
    public class SalesServiceTests
    {
        private readonly ISalesRepository _salesRepository;
        private readonly ILogger<SalesService> _logger;
        private readonly IEventPublisher _eventPublisher;
        private readonly SalesService _salesService;

        public SalesServiceTests()
        {
            _salesRepository = Substitute.For<ISalesRepository>();
            _logger = Substitute.For<ILogger<SalesService>>();
            _eventPublisher = Substitute.For<IEventPublisher>();
            _salesService = new SalesService(_salesRepository, _logger, _eventPublisher);
        }

        [Fact]
        public async Task GetSaleAsync_Should_Return_Sale_If_Found()
        {
            // Arrange
            var saleNumber = Guid.NewGuid();
            var sale = new Sale { SaleNumber = saleNumber };
            _salesRepository.GetSaleByIdAsync(saleNumber).Returns(sale);

            // Act
            var result = await _salesService.GetSaleAsync(saleNumber);

            // Assert
            result.Should().Be(sale);
        }

        [Fact]
        public async Task GetSaleAsync_Should_Return_Null_If_Not_Found()
        {
            // Arrange
            var saleNumber = Guid.NewGuid();
            _salesRepository.GetSaleByIdAsync(saleNumber).Returns((Sale)null);

            // Act
            var result = await _salesService.GetSaleAsync(saleNumber);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task CreateSaleAsync_Should_Create_Sale_And_Publish_Event()
        {
            // Arrange
            var sale = new Sale
            {
                SaleNumber = Guid.NewGuid(),
                TotalAmount = 1000m,
                Items = new List<SaleItem>()
            };

            // Act
            await _salesService.CreateSaleAsync(sale);

            // Assert
            await _salesRepository.Received(1).CreateSaleAsync(sale);
            _eventPublisher.Received(1).PublishSaleCreated(Arg.Is<SaleCreatedEvent>(e =>
                e.SaleId == sale.SaleNumber && e.TotalAmount == sale.TotalAmount));
          
        }

        [Fact]
        public async Task UpdateSaleAsync_Should_Update_Sale_And_Publish_Event()
        {
            // Arrange
            var sale = new Sale
            {
                SaleNumber = Guid.NewGuid(),
                TotalAmount = 1500m
            };

            // Act
            await _salesService.UpdateSaleAsync(sale);

            // Assert
            await _salesRepository.Received(1).UpdateSaleAsync(sale);
            _eventPublisher.Received(1).PublishSaleUpdated(Arg.Is<SaleUpdatedEvent>(e =>
                e.SaleId == sale.SaleNumber && e.NewTotalAmount == sale.TotalAmount));
        }

        [Fact]
        public async Task DeleteSaleAsync_Should_Throw_If_Sale_Not_Found()
        {
            // Arrange
            var saleNumber = Guid.NewGuid();
            _salesRepository.GetSaleByIdAsync(saleNumber).Returns((Sale)null);

            // Act
            Func<Task> act = async () => await _salesService.DeleteSaleAsync(saleNumber);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage($"Venda com número {saleNumber} não encontrada.");
        }

        [Fact]
        public async Task CancelSaleAsync_Should_Cancel_Sale_And_Publish_Event()
        {
            // Arrange
            var saleNumber = Guid.NewGuid();
            var sale = new Sale { SaleNumber = saleNumber };
            _salesRepository.GetSaleByIdAsync(saleNumber).Returns(sale);

            // Act
            await _salesService.CancelSaleAsync(saleNumber);

            // Assert
            sale.IsCanceled.Should().BeTrue();
            await _salesRepository.Received(1).UpdateSaleAsync(sale);
            _eventPublisher.Received(1).PublishSaleCanceled(Arg.Is<SaleCanceledEvent>(e => e.SaleId == saleNumber));

        }

        [Fact]
        public async Task CancelSaleItemAsync_Should_Cancel_Item_And_Publish_Event()
        {
            // Arrange
            var saleNumber = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var sale = new Sale
            {
                SaleNumber = saleNumber,
                Items = new List<SaleItem>
                {
                    new SaleItem { ProductId = productId, ProductDescription = "Test Product" }
                }
            };
            _salesRepository.GetSaleByIdAsync(saleNumber).Returns(sale);

            // Act
            await _salesService.CancelSaleItemAsync(saleNumber, productId);

            // Assert
            sale.Items[0].IsCanceled.Should().BeTrue();
            await _salesRepository.Received(1).UpdateSaleAsync(sale);
            _eventPublisher.Received(1).PublishItemCanceled(Arg.Is<ItemCanceledEvent>(e =>
                e.SaleId == saleNumber && e.ItemId == productId && e.ProductName == "Test Product"));

        }
    }
}
