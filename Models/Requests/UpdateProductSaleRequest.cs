using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;


namespace StoreAPI.Models.Requests
{
    public class UpdateProductSaleRequest
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        [Range(1, double.MaxValue, ErrorMessage = "Sold amount must be greater than 0")]
        public int SoldAmount { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal PricePerUnit { get; set; }

        [Required]
        public DateTime Date { get; set; }
    }
}
