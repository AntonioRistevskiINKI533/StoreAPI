using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;


namespace StoreAPI.Models
{
    public class SupplierPurchaseSumsView
    {
        [Key]
        public int SupplierId { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public long SumOfPurchases { get; set; }
        public int SumOfUnits { get; set; }
        public decimal SumOfTotalPurchasePrice { get; set; }
    }
}
