namespace StoreAPI.Models
{
    public class ProductSale
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int SoldAmount { get; set; }
        public decimal PricePerUnit { get; set; }
        public DateTime Date { get; set; }
    }
}
