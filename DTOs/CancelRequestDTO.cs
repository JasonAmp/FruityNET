using System;

namespace FruityNET.DTOs
{
    public class CancelRequestDTO
    {
        public Guid RequestID { get; set; }
        public Guid RequestUserID { get; set; }
        public string RequestUsername { get; set; }
        public Guid AccountID { get; set; }
        public Guid RecipientAccountID { get; set; }
        public string RecipientUsername { get; set; }


    }

    public class SendRequestDTO
    {
        public Guid RequestID { get; set; }
        public Guid RequestUserID { get; set; }
        public string RequestUsername { get; set; }
        public Guid AccountID { get; set; }
        public Guid RecipientAccountID { get; set; }
        public Guid RecipientFriendListID { get; set; }
        public string RecipientUsername { get; set; }


    }
}