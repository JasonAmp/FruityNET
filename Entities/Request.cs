using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FruityNET.Enums;

namespace FruityNET.Entities
{
    public class Request
    {
        [Key]
        public Guid Id { get; set; }

        public bool Pending { get; set; }

        public Response UserResponse { get; set; }

        public Guid RequestUserId { get; set; }

        [ForeignKey("RequestUserId")]
        public RequestUser RequestUser { get; set; }

        public string Username { get; set; }


        public Guid FriendListId { get; set; }

        [ForeignKey("FriendListId")]
        public FriendList FriendList { get; set; }


    }
}