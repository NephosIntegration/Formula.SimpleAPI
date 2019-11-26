using System;
using System.Collections.Generic;

namespace Formula.SimpleAPI.Auth
{
    public class UserClaims
    {
        public IEnumerable<ClaimVM> Claims { get; set; }
        public string UserName { get; set; }
    }
}
