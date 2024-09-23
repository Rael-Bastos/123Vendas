using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _123Vendas.Vendas.Infra.Events
{
    public class SaleUpdatedEvent
    {
        public Guid SaleId { get; set; }
        public DateTime UpdatedAt { get; set; }
        public decimal NewTotalAmount { get; set; }

        public SaleUpdatedEvent(Guid saleId, decimal newTotalAmount)
        {
            SaleId = saleId;
            UpdatedAt = DateTime.Now;
            NewTotalAmount = newTotalAmount;
        }
    }
}
