using System;
using System.Collections.Generic;

namespace Formula.SimpleAPI.Auth
{
    public class UserStateVM
    {
        public bool IsAuthenticated { get; set; }
        public string Username { get; set; }
    }
}
