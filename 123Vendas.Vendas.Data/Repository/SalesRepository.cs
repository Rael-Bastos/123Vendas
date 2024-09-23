using _123Vendas.Vendas.Data.Context;
using _123Vendas.Vendas.Data.Entity;
using _123Vendas.Vendas.Infra.Interface.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace _123Vendas.Vendas.Data.Repository
{
    public class SalesRepository : ISalesRepository
    {
        private readonly SalesDbContext _context;
        private readonly ILogger<SalesRepository> _logger;

        public SalesRepository(SalesDbContext context, ILogger<SalesRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Sale> GetSaleByIdAsync(Guid saleNumber)
        {
            _logger.LogInformation("Buscando a venda com número {SaleNumber}", saleNumber);
            var sale = await _context.Sales.Include(s => s.Items)
                                           .FirstOrDefaultAsync(s => s.SaleNumber == saleNumber);
            if (sale == null)
            {
                _logger.LogWarning("Venda com número {SaleNumber} não foi encontrada", saleNumber);
            }
            return sale;
        }

        public async Task CreateSaleAsync(Sale sale)
        {
            _logger.LogInformation("Criando nova venda para o cliente {CustomerId}", sale.CustomerId);
            _context.Sales.Add(sale);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Venda {SaleNumber} criada com sucesso", sale.SaleNumber);
        }

        public async Task UpdateSaleAsync(Sale sale)
        {
            _logger.LogInformation("Atualizando venda {SaleNumber}", sale.SaleNumber);
            _context.Sales.Attach(sale);
            _context.Entry(sale).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            _logger.LogInformation("Venda {SaleNumber} atualizada com sucesso", sale.SaleNumber);
        }

        public async Task DeleteSaleAsync(Guid saleNumber)
        {
            var sale = await GetSaleByIdAsync(saleNumber);
            if (sale != null)
            {
                _logger.LogInformation("Deletando venda {SaleNumber}", saleNumber);
                _context.Sales.Remove(sale);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Venda {SaleNumber} deletada com sucesso", saleNumber);
            }
            else
            {
                _logger.LogWarning("Venda {SaleNumber} não foi encontrada para exclusão", saleNumber);
            }
        }
    }
}
