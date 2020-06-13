using System;
using System.Collections.Generic;
using FruityNET.Enums;
using Microsoft.AspNetCore.Identity;

namespace FruityNET.Entities
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime LastActive { get; set; } = DateTime.UtcNow;
        public DateTime DateJoined { get; set; } = DateTime.UtcNow;
        public UserType UserType { get; set; }







    }
}