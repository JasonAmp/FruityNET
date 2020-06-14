using System;
using System.Collections.Generic;
using FruityNET.Enums;

namespace FruityNET.DTOs
{
    public class AdminPortalViewDTO
    {
        public string UserId { get; set; }
        public List<AccountDTO> Accounts { get; set; }
    }

    public class AccountDTO
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Location { get; set; }
        public string Occupation { get; set; }
        public DateTime LastActive { get; set; }
        public DateTime DateJoined { get; set; }
        public UserType UserType { get; set; }
        public string UserId { get; set; }

    }
}