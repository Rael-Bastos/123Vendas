using _123Vendas.Vendas.Data.Entity;
using _123Vendas.Vendas.Infra.Events;
using _123Vendas.Vendas.Infra.Interface.Events;
using _123Vendas.Vendas.Infra.Interface.Repository;
using _123Vendas.Vendas.Infra.Interface.Service;
using Microsoft.Extensions.Logging;

namespace _123Vendas.Vendas.Service.Services
{
    public class SalesService : ISalesService
    {
        private readonly ISalesRepository _salesRepository;
        private readonly ILogger<SalesService> _logger;
        private readonly IEventPublisher _eventPublisher;

        public SalesService(ISalesRepository salesRepository,
                            ILogger<SalesService> logger,
                            IEventPublisher eventPublisher)
        {
            _salesRepository = salesRepository;
            _logger = logger;
            _eventPublisher = eventPublisher;
        }

        public async Task<Sale?> GetSaleAsync(Guid saleNumber)
        {
            _logger.LogInformation("Buscando venda com número {SaleNumber}", saleNumber);
            var sale = await _salesRepository.GetSaleByIdAsync(saleNumber);

            if (sale == null)
            {
                _logger.LogWarning("Venda com número {SaleNumber} não encontrada", saleNumber);
                return null;
            }

            _logger.LogInformation("Venda {SaleNumber} encontrada com sucesso", saleNumber);
            return sale;
        }

        public async Task CreateSaleAsync(Sale sale)
        {
            _logger.LogInformation("Criando venda com número {SaleNumber}", sale.SaleNumber);

            await _salesRepository.CreateSaleAsync(sale);

            var saleEvent = new SaleCreatedEvent(sale.SaleNumber, sale.TotalAmount);
            _eventPublisher.PublishSaleCreated(saleEvent);

            _logger.LogInformation("Venda {SaleNumber} criada com sucesso", sale.SaleNumber);
        }

        public async Task UpdateSaleAsync(Sale sale)
        {
            _logger.LogInformation("Atualizando venda com número {SaleNumber}", sale.SaleNumber);

            await _salesRepository.UpdateSaleAsync(sale);

            var saleEvent = new SaleUpdatedEvent(sale.SaleNumber, sale.TotalAmount);
            _eventPublisher.PublishSaleUpdated(saleEvent);

            _logger.LogInformation("Venda {SaleNumber} atualizada com sucesso", sale.SaleNumber);
        }

        public async Task DeleteSaleAsync(Guid saleNumber)
        {
            _logger.LogInformation("Deletando venda com número {SaleNumber}", saleNumber);

            var sale = await _salesRepository.GetSaleByIdAsync(saleNumber);
            if (sale == null)
            {
                _logger.LogWarning("Venda com número {SaleNumber} não encontrada para exclusão", saleNumber);
                throw new InvalidOperationException($"Venda com número {saleNumber} não encontrada.");
            }

            await _salesRepository.DeleteSaleAsync(saleNumber);

            _logger.LogInformation("Venda {SaleNumber} deletada com sucesso", saleNumber);
        }

        public async Task CancelSaleAsync(Guid saleNumber)
        {
            _logger.LogInformation("Cancelando venda com número {SaleNumber}", saleNumber);

            var sale = await _salesRepository.GetSaleByIdAsync(saleNumber);
            if (sale == null)
            {
                _logger.LogWarning("Venda com número {SaleNumber} não encontrada para cancelamento", saleNumber);
                throw new InvalidOperationException($"Venda com número {saleNumber} não encontrada.");
            }

            sale.IsCanceled = true;
            await _salesRepository.UpdateSaleAsync(sale);

            var saleEvent = new SaleCanceledEvent(saleNumber);
            _eventPublisher.PublishSaleCanceled(saleEvent);

            _logger.LogInformation("Venda {SaleNumber} cancelada com sucesso", saleNumber);
        }

        public async Task CancelSaleItemAsync(Guid saleNumber, Guid productId)
        {
            _logger.LogInformation("Cancelando item da venda com número {SaleNumber}", saleNumber);

            var sale = await _salesRepository.GetSaleByIdAsync(saleNumber);
            if (sale == null)
            {
                _logger.LogWarning("Venda com número {SaleNumber} não encontrada para cancelamento de item", saleNumber);
                throw new InvalidOperationException($"Venda com número {saleNumber} não encontrada.");
            }

            var product = sale.Items.FirstOrDefault(x => x.ProductId == productId);
            if (product == null)
            {
                _logger.LogWarning("Produto com ID {ProductId} não encontrado na venda {SaleNumber}", productId, saleNumber);
                throw new InvalidOperationException($"Produto com ID {productId} não encontrado na venda.");
            }

            product.IsCanceled = true;
            await _salesRepository.UpdateSaleAsync(sale);

            var saleEvent = new ItemCanceledEvent(saleNumber, productId, product.ProductDescription);
            _eventPublisher.PublishItemCanceled(saleEvent);

            _logger.LogInformation("Item {ProductId} da venda {SaleNumber} cancelado com sucesso", productId, saleNumber);
        }
    }
}

