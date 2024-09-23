using _123Vendas.Vendas.Infra.Events;

namespace _123Vendas.Vendas.Infra.Interface.Events
{
    public interface IEventPublisher
    {
        void PublishSaleCreated(SaleCreatedEvent saleCreatedEvent);
        void PublishSaleUpdated(SaleUpdatedEvent saleUpdatedEvent);
        void PublishSaleCanceled(SaleCanceledEvent saleCanceledEvent);
        void PublishItemCanceled(ItemCanceledEvent itemCanceledEvent);
    }
}
