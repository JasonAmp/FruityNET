using System;
using System.Collections.Generic;
using FruityNET.Data;
using FruityNET.Entities;
using FruityNET.IEntityStore;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using FruityNET.DTOs;

namespace FruityNET.EntityStore
{
    public class PostStore : IPostStore
    {
        private ApplicationDbContext _Context;


        public PostStore(ApplicationDbContext _Context)
        {
            this._Context = _Context;
        }
        public Post AddPost(Post post)
        {
            _Context.Post.Add(post);
            _Context.SaveChanges();
            return post;
        }

        public Post EditPost(EditPostDTO editPostDTO, Post post)
        {
            post.Content = editPostDTO.Content + " " + "(Edited)";
            post.Id = editPostDTO.Id;
            _Context.SaveChanges();
            return post;
        }

        public Post DeletePost(Post post)
        {
            _Context.Post.Remove(post);
            _Context.SaveChanges();
            return post;
        }

        public Post ViewPost(Guid PostId)
        {
            var post = _Context.Post.Find(PostId);
            return post;

        }

        public Post GetById(Guid Id)
        {
            var post = _Context.Post.Find(Id);
            return post;

        }

        public List<Post> AllPostByCurrentUser(string UserId)
        {

            var PostsByUser = new List<Post>();

            foreach (var post in _Context.Post)
            {
                if (post.UserId == UserId)
                    PostsByUser.Add(post);
            }
            return PostsByUser;
        }

        public List<Post> AllPostByUser(string UserId)
        {

            var query = from x in _Context.Post
                        where (x.UserId == UserId)
                        select x;

            return query.ToList();
        }
    }
}