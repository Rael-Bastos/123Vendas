using _123Vendas.Vendas.Infra.Events;
using _123Vendas.Vendas.Infra.Interface.Events;
using Microsoft.Extensions.Logging;

namespace _123Vendas.Vendas.Service.Events
{
    public class EventPublisher : IEventPublisher
    {
        private readonly ILogger<EventPublisher> _logger;

        public EventPublisher(ILogger<EventPublisher> logger)
        {
            _logger = logger;
        }

        public void PublishSaleCreated(SaleCreatedEvent saleCreatedEvent)
        {
            _logger.LogInformation("Event: Sale Created. SaleId: {SaleId}, Date: {CreatedAt}, TotalAmount: {TotalAmount}",
                saleCreatedEvent.SaleId, saleCreatedEvent.CreatedAt, saleCreatedEvent.TotalAmount);
        }

        public void PublishSaleUpdated(SaleUpdatedEvent saleUpdatedEvent)
        {
            _logger.LogInformation("Event: Sale Updated. SaleId: {SaleId}, Date: {UpdatedAt}, New Total: {NewTotalAmount}",
                saleUpdatedEvent.SaleId, saleUpdatedEvent.UpdatedAt, saleUpdatedEvent.NewTotalAmount);
        }

        public void PublishSaleCanceled(SaleCanceledEvent saleCanceledEvent)
        {
            _logger.LogInformation("Event: Sale Canceled. SaleId: {SaleId}, Date: {CanceledAt}",
                saleCanceledEvent.SaleId, saleCanceledEvent.CanceledAt);
        }

        public void PublishItemCanceled(ItemCanceledEvent itemCanceledEvent)
        {
            _logger.LogInformation("Event: Item Canceled. SaleId: {SaleId}, ItemId: {ItemId}, Product: {ProductName}, Date: {CanceledAt}",
                itemCanceledEvent.SaleId, itemCanceledEvent.ItemId, itemCanceledEvent.ProductName, itemCanceledEvent.CanceledAt);
        }
    }
}
