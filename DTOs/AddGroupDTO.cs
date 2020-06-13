using System;
using System.ComponentModel.DataAnnotations;
using FruityNET.Enums;

namespace FruityNET.DTOs
{
    public class AddGroupDTO
    {
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public string UserId { get; set; }
        public DateTime CreatedOn { get; set; }
    }


    public class AddGroupUserDTO
    {
        public Guid Id { get; set; }

        [Required]
        public string Username { get; set; }
        public GroupUserType Type { get; set; }
        public string UserId { get; set; }
    }
}