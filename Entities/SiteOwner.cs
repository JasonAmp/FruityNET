using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FruityNET.Enums;

namespace FruityNET.Entities
{
    public class SiteOwner
    {
        [Key]
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime LastActive { get; set; } = DateTime.UtcNow;
        public DateTime DateJoined { get; set; } = DateTime.UtcNow;
        public UserType UserType { get; set; }
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }


    }
}