using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FruityNET.Entities
{
    public class Comment
    {
        [Key]
        public Guid Id { get; set; }

        public string Content { get; set; }

        public DateTime DatePosted { get; set; } = DateTime.UtcNow;
        public Guid PostId { get; set; }

        [ForeignKey("PostId")]
        public Post Post { get; set; }

        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }
    }
}