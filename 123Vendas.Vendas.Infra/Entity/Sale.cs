namespace _123Vendas.Vendas.Data.Entity
{
    public class Sale
    {
        public Guid SaleNumber { get; set; }
        public DateTime SaleDate { get; set; }
        public string CustomerId { get; set; }
        public decimal TotalAmount { get; set; }
        public string Branch { get; set; }
        public List<SaleItem> Items { get; set; }
        public bool IsCanceled { get; set; }
    }
}
