using _123Vendas.Vendas.Data.Entity;

namespace _123Vendas.Vendas.Infra.Interface.Service
{
    public interface ISalesService
    {
        Task<Sale?> GetSaleAsync(Guid saleNumber);
        Task CreateSaleAsync(Sale sale);
        Task UpdateSaleAsync(Sale sale);
        Task DeleteSaleAsync(Guid saleNumber);
        Task CancelSaleAsync(Guid saleNumber);
        Task CancelSaleItemAsync(Guid saleNumber, Guid productId);
    }
}
