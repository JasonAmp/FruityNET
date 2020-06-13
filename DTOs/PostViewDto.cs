using System;
using System.Collections.Generic;
using FruityNET.Entities;

namespace FruityNET.DTOs
{
    public class PostViewDto
    {

        public List<PostDTO> AllPosts { get; set; }

    }
    public class PostDTO
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Content { get; set; }
        public DateTime DatePosted { get; set; }
        public Guid UserId { get; set; }
        public string IdentityId { get; set; }

    }



    public class PostViewDetaisDTO
    {
        public string Username { get; set; }
        public string content { get; set; }
        public DateTime DatePosted { get; set; }
        public List<ViewCommentDTO> comments { get; set; }

    }
}