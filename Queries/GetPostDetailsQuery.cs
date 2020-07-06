using System.Collections.Generic;
using System.Linq;
using FruityNET.DTOs;
using FruityNET.Entities;
using FruityNET.IEntityStore;
using Microsoft.AspNetCore.Identity;

namespace FruityNET.Queries
{
    public class GetPostDetailsQuery
    {
        private IdentityUser CurrentUser { get; set; }
        private UserAccount OwnerOfPost { get; set; }
        private UserAccount CurrentUserAccount { get; set; }
        private Post ExistingPost;
        private IUserStore UserStore;
        private ICommentStore CommentStore;

        public GetPostDetailsQuery(IdentityUser CurrentUser, UserAccount OwnerOfPost, Post Post, IUserStore UserStore,
           ICommentStore CommentStore, UserAccount CurrentUserAccount)
        {
            this.CurrentUser = CurrentUser;
            this.OwnerOfPost = OwnerOfPost;
            this.ExistingPost = Post;
            this.UserStore = UserStore;
            this.CommentStore = CommentStore;
            this.CurrentUserAccount = CurrentUserAccount;
        }

        public PostViewDetaisDTO Handle()
        {
            var PostDTO = new PostViewDetaisDTO
            {
                Username = OwnerOfPost.Username,
                content = ExistingPost.Content,
                DatePosted = ExistingPost.DatePosted,
                comments = GetPostComments(),
                PostId = ExistingPost.Id,
                Permissions = CurrentUserAccount.UserType,
                UserID = CurrentUser.Id,
                PostUserRole = OwnerOfPost.UserType
            };
            return PostDTO;
        }

        public List<ViewCommentDTO> GetPostComments()
        {
            var comments = from x in CommentStore.GetAllComments()
                           where x.PostId == ExistingPost.Id
                           orderby x.DatePosted
                           select x;
            var ListOfComments = new List<ViewCommentDTO>();
            foreach (var comment in comments)
            {
                var OwnerOfComment = CommentStore.GetOwnerOfComment(comment.UserId);
                var CommentViewDTO = new ViewCommentDTO
                {
                    UserId = comment.UserId,
                    PostId = comment.PostId,
                    Content = comment.Content,
                    Username = OwnerOfComment.Username,
                    DatePosted = comment.DatePosted,
                    CommentId = comment.Id
                };
                ListOfComments.Add(CommentViewDTO);
            }
            return ListOfComments;
        }

    }
}