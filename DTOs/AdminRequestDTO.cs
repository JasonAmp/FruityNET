using System;

namespace FruityNET.DTOs
{
    public class AdminRequestDTO
    {
        public Guid RequestId { get; set; }
        public string Username { get; set; }
        public string UserId { get; set; }
        public string AccountID { get; set; }
        public Guid RequestorID { get; set; }
        public DateTime RequestDate { get; set; }
    }
}