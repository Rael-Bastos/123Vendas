using _123Vendas.Vendas.API.Controllers;
using _123Vendas.Vendas.Data.Entity;
using _123Vendas.Vendas.Infra.Interface.Service;
using _123Vendas.Vendas.Service.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _123Vendas.Vendas.API.Tests.Controllers
{
    public class SalesControllerTests
    {
        private readonly ISalesService _salesService;
        private readonly ILogger<SalesController> _logger;
        private readonly SalesController _controller;

        public SalesControllerTests()
        {
            _salesService = Substitute.For<ISalesService>();
            _logger = Substitute.For<ILogger<SalesController>>();
            _controller = new SalesController(_salesService, _logger);
        }

        [Fact]
        public async Task GetSale_ReturnsOk_WhenSaleExists()
        {
            // Arrange
            var saleNumber = Guid.NewGuid();
            var sale = new Sale { SaleNumber = saleNumber };
            _salesService.GetSaleAsync(saleNumber).Returns(sale);

            // Act
            var result = await _controller.GetSale(saleNumber);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult.Value.Should().Be(sale);
        }

        [Fact]
        public async Task GetSale_ReturnsNotFound_WhenSaleDoesNotExist()
        {
            // Arrange
            var saleNumber = Guid.NewGuid();
            _salesService.GetSaleAsync(saleNumber).Returns((Sale)null);

            // Act
            var result = await _controller.GetSale(saleNumber);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task CreateSale_ReturnsCreatedAtAction_WhenSaleIsCreated()
        {
            // Arrange
            var sale = new Sale { SaleNumber = Guid.NewGuid() };

            // Act
            var result = await _controller.CreateSale(sale);

            // Assert
            result.Should().BeOfType<CreatedAtActionResult>();
            var createdResult = result as CreatedAtActionResult;
            createdResult.ActionName.Should().Be(nameof(SalesController.GetSale));
            createdResult.RouteValues["saleNumber"].Should().Be(sale.SaleNumber);
            
        }

        [Fact]
        public async Task UpdateSale_ReturnsNoContent_WhenSaleIsUpdated()
        {
            // Arrange
            var saleNumber = Guid.NewGuid();
            var sale = new Sale { SaleNumber = saleNumber };

            // Act
            var result = await _controller.UpdateSale(saleNumber, sale);

            // Assert
            result.Should().BeOfType<NoContentResult>();
            
        }

        [Fact]
        public async Task DeleteSale_ReturnsNoContent_WhenSaleIsDeleted()
        {
            // Arrange
            var saleNumber = Guid.NewGuid();

            // Act
            var result = await _controller.DeleteSale(saleNumber);

            // Assert
            result.Should().BeOfType<NoContentResult>();
            
        }

        [Fact]
        public async Task CancelSale_ReturnsOk_WhenSaleIsCanceled()
        {
            // Arrange
            var saleNumber = Guid.NewGuid();

            // Act
            var result = await _controller.CancelSale(saleNumber);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult.Value.Should().Be($"Venda {saleNumber} cancelada com sucesso");
            
        }

        [Fact]
        public async Task UpdateSale_ReturnsBadRequest_WhenSaleNumberDoesNotMatch()
        {
            // Arrange
            var saleNumber = Guid.NewGuid();
            var sale = new Sale { SaleNumber = Guid.NewGuid() };

            // Act
            var result = await _controller.UpdateSale(saleNumber, sale);

            // Assert
            result.Should().BeOfType<BadRequestResult>();
            
        }

        [Fact]
        public async Task CancelSale_ReturnsNotFound_WhenSaleDoesNotExist()
        {
            // Arrange
            var saleNumber = Guid.NewGuid();
            _salesService.CancelSaleAsync(saleNumber).Throws(new InvalidOperationException($"Venda com número {saleNumber} não encontrada."));

            // Act
            var result = await _controller.CancelSale(saleNumber);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
            var notFoundResult = result as NotFoundObjectResult;
            notFoundResult.Value.Should().Be($"Venda com número {saleNumber} não encontrada.");
            
        }

        [Fact]
        public async Task CancelSale_ReturnsInternalServerError_WhenUnexpectedErrorOccurs()
        {
            // Arrange
            var saleNumber = Guid.NewGuid();
            _salesService.CancelSaleAsync(saleNumber).Throws(new Exception("Erro inesperado"));

            // Act
            var result = await _controller.CancelSale(saleNumber);

            // Assert
            result.Should().BeOfType<ObjectResult>().Which.StatusCode.Should().Be(500);
            
        }

        [Fact]
        public async Task CancelSaleItem_ReturnsOk_WhenItemIsCanceled()
        {
            // Arrange
            var saleNumber = Guid.NewGuid();
            var productId = Guid.NewGuid();

            // Act
            var result = await _controller.CancelSaleItem(saleNumber, productId);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult.Value.Should().Be($"Item {productId} da venda {saleNumber} cancelado com sucesso");
            
        }

        [Fact]
        public async Task CancelSaleItem_ReturnsNotFound_WhenSaleDoesNotExist()
        {
            // Arrange
            var saleNumber = Guid.NewGuid();
            var productId = Guid.NewGuid();
            _salesService.CancelSaleItemAsync(saleNumber, productId).Throws(new InvalidOperationException($"Venda com número {saleNumber} não encontrada."));

            // Act
            var result = await _controller.CancelSaleItem(saleNumber, productId);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
            var notFoundResult = result as NotFoundObjectResult;
            notFoundResult.Value.Should().Be($"Venda com número {saleNumber} não encontrada.");
            
        }

        [Fact]
        public async Task CancelSaleItem_ReturnsInternalServerError_WhenUnexpectedErrorOccurs()
        {
            // Arrange
            var saleNumber = Guid.NewGuid();
            var productId = Guid.NewGuid();
            _salesService.CancelSaleItemAsync(saleNumber, productId).Throws(new Exception("Erro inesperado"));

            // Act
            var result = await _controller.CancelSaleItem(saleNumber, productId);

            // Assert
            result.Should().BeOfType<ObjectResult>().Which.StatusCode.Should().Be(500);
        }
    }
}
