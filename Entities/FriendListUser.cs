using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FruityNET.Entities
{
    public class FriendUser
    {
        [Key]
        public Guid Id { get; set; }
        public string Username { get; set; }

        public Guid FriendListId { get; set; }

        [ForeignKey("FriendsListId")]
        public FriendList FriendList { get; set; }

        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }


    }
}