﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;


namespace StoreAPI.Models.Responses
{
    public class LoginResponse
    {
        public string Token { get; set; }
    }
}
