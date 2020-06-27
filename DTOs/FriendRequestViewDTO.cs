using System;
using System.Collections.Generic;
using FruityNET.Enums;

namespace FruityNET.DTOs
{
    public class FriendRequestViewDTO
    {

        public List<FriendRequestDTO> IncomingRequests { get; set; }
        public List<FriendRequestDTO> OutgoingRequests { get; set; }


        public FriendRequestViewDTO()
        {
            IncomingRequests = new List<FriendRequestDTO>();
            OutgoingRequests = new List<FriendRequestDTO>();
        }


    }

    public class FriendRequestDTO
    {

        public string Username { get; set; }
        public Guid Id { get; set; }

        public bool Pending { get; set; }

        public Response UserResponse { get; set; }

        public Guid RequestUserId { get; set; }

        public Guid FriendListId { get; set; }

        public string message { get; set; }
    }
}