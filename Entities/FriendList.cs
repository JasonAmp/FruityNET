using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FruityNET.Entities
{
    public class FriendList
    {
        [Key]
        public Guid Id { get; set; }

        public List<FriendUser> Users { get; set; }

        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

        public List<Request> Requests { get; set; }

        public FriendList()
        {
            Users = new List<FriendUser>();
            Requests = new List<Request>();
        }
    }
}