namespace StoreAPI.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string RegistrationNumber { get; set; }
        public string Name { get; set; }
        public int CompanyId { get; set; }
        public decimal Price { get; set; }
    }
}
