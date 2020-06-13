using System;
using System.Collections.Generic;
using FruityNET.DTOs;
using FruityNET.Entities;

namespace FruityNET.IEntityStore
{
    public interface IPostStore
    {
        Post AddPost(Post post);

        Post ViewPost(Guid Id);

        Post GetById(Guid Id);

        Post DeletePost(Post post);
        List<Post> AllPostByCurrentUser(string UserId);

        List<Post> AllPostByUser(string UserId);

        Post EditPost(EditPostDTO editPostDTO, Post post);
    }
}