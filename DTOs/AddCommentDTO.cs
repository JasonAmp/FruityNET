using System;
using System.ComponentModel.DataAnnotations;

namespace FruityNET.DTOs
{
    public class AddCommentDTO
    {
        public Guid PostId { get; set; }

        public string UserId { get; set; }

        [Required]
        public string Content { get; set; }


        public string PostContent { get; set; }

    }

    public class ViewCommentDTO
    {
        public Guid PostId { get; set; }

        public string UserId { get; set; }

        public string Content { get; set; }
        public string Username { get; set; }

        public DateTime DatePosted { get; set; }

    }
}