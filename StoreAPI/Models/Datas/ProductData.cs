using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace StoreAPI.Models.Datas
{
    public class ProductData
    {
        public int Id { get; set; }
        public string RegistrationNumber { get; set; }
        public string Name { get; set; }
        public int CompanyId { get; set; }
        public decimal Price { get; set; }
        public string? CompanyName { get; set; }
    }
}
