using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FruityNET.Entities
{
    public class Group
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreationDate { get; set; }
        public List<GroupUser> GroupMembers { get; set; }
        public List<GroupRequest> GroupRequests { get; set; }

        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }
        public Group()
        {
            GroupMembers = new List<GroupUser>();
            GroupRequests = new List<GroupRequest>();
        }
    }
}