using System;

namespace FruityNET.DTOs
{
    public class DeleteCommentDTO
    {
        public Guid PostID { get; set; }
        public Guid CommentID { get; set; }
        public string PostedByUsername { get; set; }
        public string UserId { get; set; }
        public string Content { get; set; }
    }
}