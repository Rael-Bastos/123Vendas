using _123Vendas.Vendas.Data.Entity;

namespace _123Vendas.Vendas.Infra.Interface.Repository
{
    public interface ISalesRepository
    {
        Task<Sale> GetSaleByIdAsync(Guid saleNumber);
        Task CreateSaleAsync(Sale sale);
        Task UpdateSaleAsync(Sale sale);
        Task DeleteSaleAsync(Guid saleNumber);
    }
}
