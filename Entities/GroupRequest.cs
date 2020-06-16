using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FruityNET.Enums;

namespace FruityNET.Entities
{
    public class GroupRequest
    {
        [Key]
        public Guid Id { get; set; }

        public bool Pending { get; set; }

        public DateTime RequestDate { get; set; }

        public Guid RequestUserId { get; set; }

        [ForeignKey("RequestUserId")]
        public GroupRequestUser RequestUser { get; set; }

        public Guid GroupId { get; set; }

        [ForeignKey("GroupId")]
        public Group Group { get; set; }
    }
}