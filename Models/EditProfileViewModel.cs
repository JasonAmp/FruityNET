using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using FruityNET.Entities;
using FruityNET.Enums;

namespace FruityNET.Models
{
    public class EditProfileViewModel
    {
        public string UserId { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Username { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Location { get; set; }
        [Required]
        public string Occupation { get; set; }
    }


    public class ProfileViewModel
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Location { get; set; }
        public string Occupation { get; set; }
        public DateTime LastActive { get; set; }
        public DateTime JoinDate { get; set; }
        public UserType UserType { get; set; }
        public List<FriendDTO> Friends { get; set; }
        public List<Group> Groups { get; set; }
        public int NotificationCount { get; set; }
        public Request PendingRequest { get; set; }
        public Guid FriendListID { get; set; }
        public FriendUser ExistingFriend { get; set; }
        public Request RequestToCurrent { get; set; }

        public ProfileViewModel()
        {
            Friends = new List<FriendDTO>();
        }


    }
    public class FriendDTO
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public Guid FriendListId { get; set; }
        public string UserId { get; set; }
        public Guid AccountId { get; set; }



    }
}