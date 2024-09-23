using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _123Vendas.Vendas.Infra.Events
{
    public class ItemCanceledEvent
    {
        public Guid SaleId { get; set; }
        public Guid ItemId { get; set; }
        public string ProductName { get; set; }
        public DateTime CanceledAt { get; set; }

        public ItemCanceledEvent(Guid saleId, Guid itemId, string productName)
        {
            SaleId = saleId;
            ItemId = itemId;
            ProductName = productName;
            CanceledAt = DateTime.Now;
        }
    }
}
