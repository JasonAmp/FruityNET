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
using AutoMapper;
using FruityNET.Mapper;

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
        private readonly IMapper _mapper;

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
            _mapper = new MapperConfiguration(cfg => cfg.AddProfile<EntityMapper>()).CreateMapper();
        }


        public ProfileViewModel Handle()
        {
            if (_CurrentUser is null)
                throw new DomainException(ErrorMessages.NotSignedIn);

            var existingAccount = _UserStore.GetById(AccountId);
            if (existingAccount is null || existingAccount.AccountStatus.Equals(Status.Suspended)
            || existingAccount.AccountStatus.Equals(Status.Inactive))
                throw new DomainException(ErrorMessages.UserDoesNotExist);

            FriendList FriendList = _FriendListStore.GetFriendListOfUser(existingAccount.UserId);
            FriendList CurrentFriendList = _FriendListStore.GetFriendListOfUser(_CurrentUser.Id);
            var FriendsOfUser = _FriendListStore.GetFriendsOfUser(FriendList.Id);
            var FriendsOfCurrent = _FriendListStore.GetFriendsOfUser(CurrentFriendList.Id);

            List<Group> GroupsWithUser = _GroupStore.GetGroupsWithUser(existingAccount.UserId);
            List<Request> FriendRequests = _FriendListStore.GetIncomingFriendRequests(FriendList.Id);
            List<Request> CurrentUserRequests = _FriendListStore.GetIncomingFriendRequests(CurrentFriendList.Id);
            FriendUser existingFriend = FriendsOfCurrent.FirstOrDefault(x => x.UserId.Equals(existingAccount.UserId));

            var ProfileViewModel = CreateViewModel(existingAccount, GroupsWithUser, existingFriend, FriendList, FriendRequests,
            CurrentFriendList, CurrentUserRequests);
            ProfileViewModel.Friends = GetFriendDTOs(FriendsOfUser);
            return ProfileViewModel;
        }
        public ProfileViewModel CreateViewModel(UserAccount existingAccount, List<Group> GroupsWithUser, FriendUser existingFriend,
        FriendList FriendList, List<Request> FriendRequests, FriendList CurrentFriendList, List<Request> CurrentUserRequests)
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
                ExistingFriend = existingFriend,
                RequestToCurrent = CurrentUserRequests.FirstOrDefault(x => x.Username.Equals(existingAccount.Username))
            };
            return ProfileViewModel;
        }

        public List<FriendDTO> GetFriendDTOs(List<FriendUser> Friends)
        {
            var FriendDTOs = _mapper.Map<List<FriendDTO>>(Friends);

            foreach (var friend in FriendDTOs)
            {
                var account = _UserStore.GetByUsername(friend.Username);
                friend.AccountId = account.Id;
            }
            return FriendDTOs;
        }
    }
}