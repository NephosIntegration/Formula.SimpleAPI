using System;
using System.Collections.Generic;

namespace Formula.SimpleAPI.Auth
{
    public static class UserRepository
    {
        public static List<AppUser> Users;
    
        static UserRepository()
        {
            Users = new List<AppUser>();
        }
    }
}
