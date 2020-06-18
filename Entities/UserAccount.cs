using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FruityNET.Enums;

namespace FruityNET.Entities
{
    public class UserAccount
    {
        [Key]
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Location { get; set; }
        public string Occupation { get; set; }
        public Status AccountStatus { get; set; }
        public DateTime LastActive { get; set; }
        public DateTime DateJoined { get; set; } = DateTime.Now;
        public UserType UserType { get; set; }
        public List<Group> Groups { get; set; }
        public List<Post> Posts { get; set; }
        public List<FriendUser> FriendList { get; set; }
        public List<Request> IncomingRequests { get; set; }
        public List<Request> OutgoingRequests { get; set; }
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

        public UserAccount()
        {
            Posts = new List<Post>();
            FriendList = new List<FriendUser>();
            Groups = new List<Group>();
            IncomingRequests = new List<Request>();
            OutgoingRequests = new List<Request>();

        }
    }
}