using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FruityNET.Entities
{
    public class Post
    {
        [Key]
        public Guid Id { get; set; }
        public string Content { get; set; }
        public DateTime DatePosted { get; set; } = DateTime.UtcNow;
        public List<Comment> Comments { get; set; }
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }
        public Post()
        {
            Comments = new List<Comment>();
        }
    }
}