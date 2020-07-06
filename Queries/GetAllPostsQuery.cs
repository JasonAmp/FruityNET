using System.Collections.Generic;
using System.Linq;
using FruityNET.DTOs;
using FruityNET.Entities;
using FruityNET.Enums;
using FruityNET.IEntityStore;
using Microsoft.AspNetCore.Identity;

namespace FruityNET.Queries
{
    public class GetAllPostsQuery
    {
        private IdentityUser CurrentUser;
        private UserAccount ExistingAccount;
        private IFriendsListStore _FriendListStore;
        private INotificationBox _NotificationBox;
        private IUserStore _UserStore;

        private IPostStore _PostStore;


        public GetAllPostsQuery(IdentityUser CurrentUser, UserAccount ExistingAccount, IFriendsListStore _FriendListStore,
         INotificationBox _NotificationBox, IUserStore _UserStore, IPostStore _PostStore)
        {
            this.CurrentUser = CurrentUser;
            this.ExistingAccount = ExistingAccount;
            this._FriendListStore = _FriendListStore;
            this._NotificationBox = _NotificationBox;
            this._UserStore = _UserStore;
            this._PostStore = _PostStore;
        }

        public PostViewDto Handle()
        {
            var FriendList = _FriendListStore.GetFriendListOfUser(CurrentUser.Id);
            FriendList.Users = _FriendListStore.GetFriendsOfUser(FriendList.Id);

            var Notifications = _NotificationBox.GetUserNotifications(ExistingAccount.Username);
            var FriendRequests = _FriendListStore.GetIncomingFriendRequests(FriendList.Id);
            var Posts = GetCurrentUserPosts().Concat(GetAllFriendPosts(FriendList)).ToList();

            var postViewDTO = new PostViewDto
            {
                FriendRequestCount = FriendRequests.Count,
                NotificationCount = Notifications.Count,
                Permissions = ExistingAccount.UserType,
                AllPosts = Posts
            };

            return postViewDTO;
        }

        public List<PostDTO> GetAllFriendPosts(FriendList friendList)
        {
            var PostDTOs = new List<PostDTO>();
            foreach (var friend in friendList.Users)
            {
                var FriendAccount = _UserStore.GetByIdentityUserId(friend.UserId);
                if (FriendAccount.AccountStatus.Equals(Status.Active))
                {
                    var AllPosts = _PostStore.AllPostByUser(friend.UserId);
                    foreach (var post in AllPosts)
                    {
                        var PostDTO = new PostDTO
                        {
                            Id = post.Id,
                            Content = post.Content,
                            DatePosted = post.DatePosted,
                            Username = friend.Username,
                            UserId = FriendAccount.Id,
                            Role = FriendAccount.UserType
                        };
                        PostDTOs.Add(PostDTO);
                    }
                }
            }
            return PostDTOs;
        }

        public List<PostDTO> GetCurrentUserPosts()
        {
            var PostDTOs = new List<PostDTO>();
            List<Post> AllPostsOfCurrent = _PostStore.AllPostByCurrentUser(CurrentUser.Id);
            foreach (var post in AllPostsOfCurrent)
            {
                var PostDTO = new PostDTO
                {
                    Id = post.Id,
                    Content = post.Content,
                    DatePosted = post.DatePosted,
                    Username = ExistingAccount.Username,
                    UserId = ExistingAccount.Id,
                    IdentityId = CurrentUser.Id
                };
                PostDTOs.Add(PostDTO);
            }
            return PostDTOs;
        }
    }
}