using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _123Vendas.Vendas.Infra.Events
{
    public class SaleCanceledEvent
    {
        public Guid SaleId { get; set; }
        public DateTime CanceledAt { get; set; }

        public SaleCanceledEvent(Guid saleId)
        {
            SaleId = saleId;
            CanceledAt = DateTime.Now;
        }
    }
}
