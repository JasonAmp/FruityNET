using System;
using System.Collections.Generic;
using System.Linq;
using FruityNET.DTOs;
using FruityNET.Entities;
using FruityNET.Enums;
using FruityNET.Exceptions;
using FruityNET.IEntityStore;
using Microsoft.AspNetCore.Identity;

namespace FruityNET.Queries
{
    public class GetAllUsersQuery
    {
        private readonly IUserStore _UserStore;
        private readonly IFriendsListStore _FriendListStore;
        private readonly IRequestStore _RequestStore;
        private IdentityUser CurrentUser;
        private readonly UserAccount existingAccount;
        private readonly SignInManager<User> signInManager;


        public GetAllUsersQuery(IUserStore _UserStore, IFriendsListStore _FriendListStore,
         IRequestStore _RequestStore, IdentityUser CurrentUser, UserAccount existingAccount,
         SignInManager<User> signInManager)
        {
            this._RequestStore = _RequestStore;
            this._FriendListStore = _FriendListStore;
            this.CurrentUser = CurrentUser;
            this._UserStore = _UserStore;
            this.existingAccount = existingAccount;
            this.signInManager = signInManager;
        }

        public SearchUserDTO Handle()
        {


            if (existingAccount.AccountStatus.Equals(Status.Suspended))
                signInManager.SignOutAsync();

            var AllUsers = _UserStore.GetAccounts();
            var FriendList = _FriendListStore.GetFriendListOfUser(existingAccount.UserId);
            var CurrentUserRequests = _RequestStore.PendingRequest(FriendList.Id);

            return new SearchUserDTO()
            {
                Users = GetUserDTOs(AllUsers, FriendList, CurrentUserRequests),
                InviteCount = CurrentUserRequests.Count
            };

        }
        public List<SearchUserResultDTO> GetUserDTOs(List<UserAccount> AllUsers, FriendList FriendList, List<Request> CurrentUserRequests)
        {
            List<SearchUserResultDTO> AllUserDTOs = new List<SearchUserResultDTO>();
            foreach (var user in AllUsers)
            {
                var ResultFriendList = _FriendListStore.GetFriendListOfUser(user.UserId);
                var areFriends = _FriendListStore.IsFriendsOfUser(FriendList.Id, user.UserId);
                var ResultRequests = _RequestStore.PendingRequest(ResultFriendList.Id);
                var SentByCurrent = ResultRequests.FirstOrDefault(x => x.Username == CurrentUser.UserName);
                var SentByResult = CurrentUserRequests.FirstOrDefault(x => x.Username == user.Username);

                if (user.AccountStatus.Equals(Status.Active))
                {
                    var SearchResultDTO = new SearchUserResultDTO()
                    {
                        Id = user.Id,
                        Firstname = user.FirstName,
                        Lastname = user.LastName,
                        Username = user.Username,
                        UserType = user.UserType,
                        UserId = user.UserId,
                        isFriendsOfCurrentUser = (areFriends is true) ? true : false,
                        ResultUserFriendListID = ResultFriendList.Id,
                        RequestId = (SentByCurrent != null) ? SentByCurrent.Id : new Guid(),
                        RequestIsPending = (areFriends is false && (SentByCurrent != null || SentByResult != null)),
                    };
                    if (SentByCurrent != null)
                        SearchResultDTO.Request = SentByCurrent;

                    if (SentByResult != null)
                        SearchResultDTO.Request = SentByResult;

                    AllUserDTOs.Add(SearchResultDTO);
                }
            }
            return AllUserDTOs;
        }


    }
}