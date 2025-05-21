using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;


namespace StoreAPI.Models.Requests
{
    public class AddCompanyRequest
    {
        [Required]
        [StringLength(500, MinimumLength = 1, ErrorMessage = "Name must be between 1 and 500 characters")]
        public string Name { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 5, ErrorMessage = "Address must be between 5 and 200 characters")]
        public string Address { get; set; }

        [Required]
        [Phone(ErrorMessage = "Invalid phone")]
        public string Phone { get; set; }
    }
}
