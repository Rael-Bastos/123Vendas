namespace _123Vendas.Vendas.Data.Entity
{
    public class SaleItem
    {
        public Guid SaleNumber { get; set; } 
        public Guid ProductId { get; set; } 
        public string ProductDescription { get; set; } 
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Discount { get; set; }
        public decimal TotalPrice { get; set; }
        public bool IsCanceled { get; set; } = false;
    }
}
