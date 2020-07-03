using System;
using System.Collections.Generic;
using System.Linq;
using FruityNET.DTOs;
using FruityNET.Entities;
using FruityNET.IEntityStore;
using Microsoft.AspNetCore.Identity;

namespace FruityNET.Queries
{
    public class GetSearchResultQuery
    {
        private readonly IUserStore _UserStore;
        private readonly IFriendsListStore _FriendListStore;
        private readonly IRequestStore _RequestStore;
        private IdentityUser CurrentUser;
        private UserAccount ResultUser;
        private SearchUserDTO SearchUserDTO;

        public GetSearchResultQuery(IUserStore _UserStore, IFriendsListStore _FriendListStore,
        IRequestStore _RequestStore, IdentityUser CurrentUser, UserAccount ResultUser, SearchUserDTO SearchUserDTO)
        {
            this._RequestStore = _RequestStore;
            this._FriendListStore = _FriendListStore;
            this.CurrentUser = CurrentUser;
            this._UserStore = _UserStore;
            this.ResultUser = ResultUser;
            this.SearchUserDTO = SearchUserDTO;
        }

        public SearchUserDTO Handle()
        {
            SearchUserDTO.Users = new List<SearchUserResultDTO>();

            if (ResultUser.UserId.Equals(CurrentUser.Id))
            {
                var CurrentUserAsResult = new SearchUserResultDTO
                {
                    Id = ResultUser.Id,
                    Firstname = ResultUser.FirstName,
                    Lastname = ResultUser.LastName,
                    Username = ResultUser.Username,
                    UserType = ResultUser.UserType,
                    UserId = ResultUser.UserId,
                };
                SearchUserDTO.Users.Add(CurrentUserAsResult);
            }
            else
            {
                SearchUserDTO.Users.Add(GetSearchResult(CurrentUser, ResultUser));
            }
            return SearchUserDTO;
        }

        protected SearchUserResultDTO GetSearchResult(IdentityUser currentUser, UserAccount Result)
        {
            UserAccount CurrentUserAccount = _UserStore.GetByIdentityUserId(currentUser.Id);
            FriendList FriendList = _FriendListStore.GetFriendListOfUser(CurrentUserAccount.UserId);
            FriendList ResultFriendList = _FriendListStore.GetFriendListOfUser(Result.UserId);
            bool areFriends = _FriendListStore.IsFriendsOfUser(FriendList.Id, Result.UserId);
            List<Request> ResultRequests = _RequestStore.PendingRequest(ResultFriendList.Id);
            List<Request> CurrentUserRequests = _RequestStore.PendingRequest(FriendList.Id);
            Request SentByCurrent = ResultRequests.FirstOrDefault(x => x.Username == currentUser.UserName);
            Request SentByResult = CurrentUserRequests.FirstOrDefault(x => x.Username == Result.Username);

            var SearchResultDTO = new SearchUserResultDTO
            {
                Id = Result.Id,
                Firstname = Result.FirstName,
                Lastname = Result.LastName,
                Username = Result.Username,
                UserType = Result.UserType,
                UserId = Result.UserId,
                isFriendsOfCurrentUser = (areFriends is true) ? true : false,
                ResultUserFriendListID = ResultFriendList.Id,
                RequestIsPending = (areFriends is false && (SentByCurrent != null || SentByResult != null)),
                RequestId = (SentByCurrent != null) ? SentByCurrent.Id : new Guid(),
            };
            GetFriendInvites(SentByCurrent, SentByResult, SearchResultDTO);
            return SearchResultDTO;
        }
        protected void GetFriendInvites(Request SentByCurrent, Request SentByResult, SearchUserResultDTO SearchResultDTO)
        {
            SearchResultDTO.Request = (SentByCurrent != null) ? SentByCurrent : null;
            SearchResultDTO.Request = (SentByResult != null) ? SentByResult : null;
        }

    }
}