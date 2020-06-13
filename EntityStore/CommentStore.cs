using System;
using System.Collections.Generic;
using System.Linq;
using FruityNET.Data;
using FruityNET.Entities;
using FruityNET.IEntityStore;

using Microsoft.AspNetCore.Identity;

namespace FruityNET.EntityStore
{
    public class CommentStore : ICommentStore
    {
        private ApplicationDbContext _Context;
        private readonly UserManager<User> userManager;


        public CommentStore(ApplicationDbContext _Context,
        UserManager<User> userManager)
        {
            this._Context = _Context;
            this.userManager = userManager;
        }
        public Comment AddComment(Comment comment)
        {
            _Context.Comment.Add(comment);
            _Context.SaveChanges();
            return comment;
        }

        public Comment DeleteComment(Comment comment)
        {
            _Context.Comment.Remove(comment);
            _Context.SaveChanges();
            return comment;
        }

        public List<Comment> GetAllComments()
        {
            var Comments = new List<Comment>();
            foreach (var comment in _Context.Comment)
            {
                Comments.Add(comment);
            }
            return Comments;
        }

        public UserAccount GetOwnerOfComment(string UserId)
        {
            var User = _Context.Account.FirstOrDefault(x => x.UserId == UserId);
            return User;
        }
    }
}