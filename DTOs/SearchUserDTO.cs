using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using FruityNET.Entities;
using FruityNET.Enums;

namespace FruityNET.DTOs
{
    public class SearchUserDTO
    {
        [Required]
        public string Username { get; set; }

        public List<SearchUserResultDTO> Users { get; set; }

        public SearchUserDTO()
        {
            Users = new List<SearchUserResultDTO>();
        }
    }

    public class SearchUserResultDTO
    {
        public Guid Id { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Username { get; set; }
        public UserType UserType { get; set; }
        public Guid ResultUserFriendListID { get; set; }
        public string UserId { get; set; }
        public bool isFriendsOfCurrentUser { get; set; }
        public bool RequestIsPending { get; set; }
        public Guid RequestId { get; set; }
    }
}