using System;
using System.Collections.Generic;
using FruityNET.DTOs;

namespace FruityNET.Models
{
    public class GroupRequestsViewModel
    {
        public Guid GroupID { get; set; }
        public string GroupOwner { get; set; }
        public string OwnerIdentityUserID { get; set; }

        public List<GroupRequestDTO> requests { get; set; }

        public GroupRequestsViewModel()
        {
            requests = new List<GroupRequestDTO>();
        }
    }
}