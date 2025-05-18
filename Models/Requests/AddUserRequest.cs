using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;


namespace StoreAPI.Models.Requests
{
    public class AddUserRequest
    {
        [StringLength(20, MinimumLength = 5, ErrorMessage = "Username must be between 5 and 20 characters.")]
        public string Username { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }

        [MinLength(8, ErrorMessage = "Password must be at least 8 characters.")]
        public string Password { get; set; }
    }
}
