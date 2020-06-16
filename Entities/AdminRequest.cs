using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FruityNET.Entities
{
    public class AdminRequest
    {
        [Key]
        public Guid Id { get; set; }
        public bool Pending { get; set; }
        public DateTime RequestDate { get; set; }
        public Guid AdminRequestorId { get; set; }

        [ForeignKey("AdminRequestorId")]
        public AdminRequestor AdminRequestor { get; set; }
        public string Username { get; set; }
        public Guid ApprovalBoxId { get; set; }

        [ForeignKey("ApprovalBoxId")]
        public AdminApprovalBox ApprovalBox { get; set; }
    }

    public class AdminRequestor
    {
        [Key]
        public Guid Id { get; set; }
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }
        public string Username { get; set; }


    }
}