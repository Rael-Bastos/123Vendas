using _123Vendas.Vendas.Data.Entity;
using _123Vendas.Vendas.Infra.Events;
using _123Vendas.Vendas.Infra.Interface.Events;
using _123Vendas.Vendas.Infra.Interface.Repository;
using _123Vendas.Vendas.Infra.Interface.Service;
using Microsoft.AspNetCore.Mvc;

namespace _123Vendas.Vendas.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SalesController : ControllerBase
    {
        private readonly ISalesService _salesService;
        private readonly ILogger<SalesController> _logger;

        public SalesController(ISalesService salesService, ILogger<SalesController> logger)
        {
            _salesService = salesService;
            _logger = logger;
        }

        [HttpGet("{saleNumber}")]
        public async Task<IActionResult> GetSale(Guid saleNumber)
        {
            _logger.LogInformation("Recebida requisição para obter a venda {SaleNumber}", saleNumber);
            var sale = await _salesService.GetSaleAsync(saleNumber);
            if (sale == null)
            {
                _logger.LogWarning("Venda {SaleNumber} não encontrada", saleNumber);
                return NotFound();
            }
            return Ok(sale);
        }

        [HttpPost]
        public async Task<IActionResult> CreateSale(Sale sale)
        {
            _logger.LogInformation("Recebida requisição para criar uma nova venda");
            await _salesService.CreateSaleAsync(sale);
            return CreatedAtAction(nameof(GetSale), new { saleNumber = sale.SaleNumber }, sale);
        }

        [HttpPut("{saleNumber}")]
        public async Task<IActionResult> UpdateSale(Guid saleNumber, Sale sale)
        {
            if (saleNumber != sale.SaleNumber)
            {
                _logger.LogWarning("Número da venda {SaleNumber} não corresponde ao número na requisição", saleNumber);
                return BadRequest();
            }

            _logger.LogInformation("Recebida requisição para atualizar a venda {SaleNumber}", saleNumber);
            await _salesService.UpdateSaleAsync(sale);
            return NoContent();
        }

        [HttpDelete("{saleNumber}")]
        public async Task<IActionResult> DeleteSale(Guid saleNumber)
        {
            _logger.LogInformation("Recebida requisição para deletar a venda {SaleNumber}", saleNumber);
            await _salesService.DeleteSaleAsync(saleNumber);
            return NoContent();
        }

        [HttpPost("{saleNumber}/cancel")]
        public async Task<IActionResult> CancelSale(Guid saleNumber)
        {
            _logger.LogInformation("Recebida requisição para cancelar venda com número {SaleNumber}", saleNumber);

            try
            {
                await _salesService.CancelSaleAsync(saleNumber);
                _logger.LogInformation("Venda {SaleNumber} cancelada com sucesso", saleNumber);
                return Ok($"Venda {saleNumber} cancelada com sucesso");
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Erro ao cancelar venda com número {SaleNumber}", saleNumber);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao cancelar venda com número {SaleNumber}", saleNumber);
                return StatusCode(500, new { error = "Erro Guiderno ao processar a solicitação" });
            }
        }

        [HttpPost("{saleNumber}/items/{productId}/cancel")]
        public async Task<IActionResult> CancelSaleItem(Guid saleNumber, Guid productId)
        {
            _logger.LogInformation("Recebida requisição para cancelar item {ProductId} da venda com número {SaleNumber}", productId, saleNumber);

            try
            {
                await _salesService.CancelSaleItemAsync(saleNumber, productId);
                _logger.LogInformation("Item {ProductId} da venda {SaleNumber} cancelado com sucesso", productId, saleNumber);
                return Ok($"Item {productId} da venda {saleNumber} cancelado com sucesso");
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Erro ao cancelar item {ProductId} da venda {SaleNumber}", productId, saleNumber);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao cancelar item {ProductId} da venda {SaleNumber}", productId, saleNumber);
                return StatusCode(500, new { error = "Erro Guiderno ao processar a solicitação" });
            }
        }
    }
}
