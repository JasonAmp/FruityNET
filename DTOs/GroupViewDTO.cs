using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using FruityNET.Enums;

namespace FruityNET.DTOs
{
    public class GroupDetailsDTO
    {
        public string CurrentUsername { get; set; }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreationDate { get; set; }
        public string GroupOwner { get; set; }
        public List<GroupMemberDTO> GroupMembers { get; set; }
        public GroupDetailsDTO()
        {
            GroupMembers = new List<GroupMemberDTO>();
        }
    }

    public class EditGroupDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }

    }
    public class GroupMemberDTO
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public GroupUserType Type { get; set; }
        public Guid GroupId { get; set; }
        public string UserId { get; set; }
    }

}