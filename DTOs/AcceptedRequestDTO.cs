using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FruityNET.Data
{
    public class AcceptedRequestDTO
    {
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public string Username { get; set; }
    }
}