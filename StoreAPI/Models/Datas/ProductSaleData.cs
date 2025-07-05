using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace StoreAPI.Models.Datas
{
    public class ProductSaleData
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int SoldAmount { get; set; }
        public decimal PricePerUnit { get; set; }
        public DateTime Date { get; set; }
        public string? ProductName { get; set; }
    }
}
