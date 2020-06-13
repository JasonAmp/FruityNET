using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FruityNET.Enums;

namespace FruityNET.Entities
{
    public class GroupUser
    {
        [Key]
        public Guid Id { get; set; }

        public string Username { get; set; }

        public GroupUserType Type { get; set; }

        public Guid GroupId { get; set; }

        [ForeignKey("GroupId")]
        public Group Group { get; set; }

        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }
    }
}