using System;
using System.Collections.Generic;
using System.Linq;
using FruityNET.Controllers;
using FruityNET.Data;
using FruityNET.Entities;
using FruityNET.EntityStore;
using FruityNET.Enums;
using FruityNET.Exceptions;
using FruityNET.IEntityStore;
using FruityNET.Models;
using FruityNET.ParameterStrings;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace FruityNET.Queries
{
    public class GetUserProfileQuery
    {
        public Guid AccountId { get; set; }
        private readonly UserManager<User> _UserManager;
        private readonly IUserStore _UserStore;
        private readonly IFriendsListStore _FriendListStore;
        private readonly IRequestStore _RequestStore;
        private readonly IGroupStore _GroupStore;
        private readonly ILogger<AccountsController> _Logger;
        private readonly UserAccount _ExistingAccount;
        private readonly IdentityUser _CurrentUser;

        public GetUserProfileQuery(Guid Id, UserManager<User> UserManager,
         IUserStore UserStore, IFriendsListStore FriendListStore, IRequestStore RequestStore,
          IGroupStore GroupStore, ILogger<AccountsController> Logger,
          UserAccount existingAccount, IdentityUser Currentuser)
        {
            this.AccountId = Id;
            _UserManager = UserManager;
            _UserStore = UserStore;
            _FriendListStore = FriendListStore;
            _RequestStore = RequestStore;
            _GroupStore = GroupStore;
            _Logger = Logger;
            _CurrentUser = Currentuser;
            _ExistingAccount = existingAccount;
        }


        public ProfileViewModel Handle()
        {
            if (_CurrentUser is null)
                throw new DomainException(ErrorMessages.NotSignedIn);

            var existingAccount = _UserStore.GetById(AccountId);
            if (existingAccount is null || existingAccount.AccountStatus.Equals(Status.Suspended)
            || existingAccount.AccountStatus.Equals(Status.Inactive))
                throw new DomainException(ErrorMessages.UserDoesNotExist);

            var FriendList = _FriendListStore.GetFriendListOfUser(existingAccount.UserId);
            var FriendUsers = _FriendListStore.GetFriendsOfUser(FriendList.Id);
            var GroupsWithUser = _GroupStore.GetGroupsWithUser(existingAccount.UserId);
            var FriendRequests = _FriendListStore.GetIncomingFriendRequests(FriendList.Id);
            var existingFriend = FriendUsers.FirstOrDefault(x => x.UserId.Equals(_CurrentUser.Id));

            var ProfileViewModel = CreateViewModel(existingAccount, GroupsWithUser, existingFriend, FriendList, FriendRequests);

            ProfileViewModel.Friends = GetFriendDTOs(FriendUsers);

            return ProfileViewModel;
        }
        public ProfileViewModel CreateViewModel(UserAccount existingAccount, List<Group> GroupsWithUser, FriendUser existingFriend,
        FriendList FriendList, List<Request> FriendRequests)
        {
            var ProfileViewModel = new ProfileViewModel
            {
                FirstName = existingAccount.FirstName,
                LastName = existingAccount.LastName,
                Location = existingAccount.Location,
                Email = existingAccount.Email,
                Username = existingAccount.Username,
                Occupation = existingAccount.Occupation,
                UserType = existingAccount.UserType,
                LastActive = existingAccount.LastActive,
                JoinDate = existingAccount.DateJoined,
                Groups = GroupsWithUser,
                PendingRequest = FriendRequests.FirstOrDefault(x => x.Username.Equals(_CurrentUser.UserName)),
                FriendListID = FriendList.Id,
                ExistingFriend = existingFriend
            };
            return ProfileViewModel;
        }

        public List<FriendDTO> GetFriendDTOs(List<FriendUser> Friends)
        {
            var FriendDTOs = new List<FriendDTO>();
            foreach (var friend in Friends)
            {
                var account = _UserStore.GetByUsername(friend.Username);
                var FriendDTO = new FriendDTO()
                {
                    Id = friend.Id,
                    UserId = friend.UserId,
                    Username = friend.Username,
                    AccountId = account.Id
                };
                FriendDTOs.Add(FriendDTO);
            }
            return FriendDTOs;
        }
    }
}