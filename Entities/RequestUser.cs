using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FruityNET.Entities
{
    public class RequestUser
    {
        [Key]
        public Guid Id { get; set; }
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

        public string Username { get; set; }


    }
}