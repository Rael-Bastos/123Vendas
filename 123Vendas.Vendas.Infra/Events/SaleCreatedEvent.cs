using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _123Vendas.Vendas.Infra.Events
{
    public class SaleCreatedEvent
    {
        public Guid SaleId { get; set; }
        public DateTime CreatedAt { get; set; }
        public decimal TotalAmount { get; set; }

        public SaleCreatedEvent(Guid saleId,decimal totalAmount)
        {
            SaleId = saleId;
            CreatedAt = DateTime.Now;
            TotalAmount = totalAmount;
        }
    }
}
