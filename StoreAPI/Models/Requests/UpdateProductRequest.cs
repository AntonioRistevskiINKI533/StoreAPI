using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;


namespace StoreAPI.Models.Requests
{
    public class UpdateProductRequest
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [StringLength(500, MinimumLength = 1, ErrorMessage = "Name must be between 1 and 500 characters")]
        public string Name { get; set; }

        [Required]
        public int CompanyId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }
    }
}
