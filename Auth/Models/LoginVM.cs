using System;
using System.ComponentModel.DataAnnotations;

namespace Formula.SimpleAPI.Auth
{
    public class LoginVM
    {
        public string UserName { get; set; }
    
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
