using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;


namespace StoreAPI.Models.Requests
{
    public class UpdateUserProfileRequest
    {
        public string Username { get; set; }
        public string Email { get; set; }
    }
}
