using System;
using System.ComponentModel.DataAnnotations;

namespace FruityNET.DTOs
{
    public class AddPostDTO
    {
        [Required]
        public string Content { get; set; }
        public Guid UserId { get; set; }
    }

    public class EditPostDTO
    {
        [Required]
        public string Content { get; set; }
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

    }
}