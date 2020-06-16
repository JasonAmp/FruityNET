using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FruityNET.Entities
{
    public class AdminApprovalBox
    {
        [Key]
        public Guid Id { get; set; }

        public Guid SiteOwnerId { get; set; }

        [ForeignKey("SiteOwnerId")]
        public SiteOwner SiteOwner { get; set; }

        public List<AdminRequest> AdminApprovalRequests;

        public AdminApprovalBox()
        {
            AdminApprovalRequests = new List<AdminRequest>();
        }



    }
}