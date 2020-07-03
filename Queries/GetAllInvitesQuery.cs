using System.Collections.Generic;
using System.Linq;
using FruityNET.DTOs;
using FruityNET.Entities;
using FruityNET.IEntityStore;
using Microsoft.AspNetCore.Identity;

namespace FruityNET.Queries
{
    public class GetAllInvitesQuery
    {
        private readonly IRequestStore RequestStore;
        private readonly IFriendsListStore FriendListStore;
        private readonly IUserStore UserStore;
        private IdentityUser CurrentUser;
        private UserAccount CurrentAccount;

        public GetAllInvitesQuery(IRequestStore RequestStore, IUserStore UserStore,
        IdentityUser CurrentUser, UserAccount CurrentAccount, IFriendsListStore FriendListStore)
        {
            this.RequestStore = RequestStore;
            this.UserStore = UserStore;
            this.CurrentUser = CurrentUser;
            this.CurrentAccount = CurrentAccount;
            this.FriendListStore = FriendListStore;
        }

        public FriendRequestViewDTO Handle()
        {
            var FriendList = FriendListStore.GetFriendListOfUser(CurrentAccount.UserId);
            var IncomingRequests = RequestStore.GetAllRequests().FindAll(x => x.FriendListId == FriendList.Id);
            var CurrentAsRequestUser = RequestStore.GetAllRequestUsers().FirstOrDefault(x => x.UserId == CurrentUser.Id);
            var OutgoingRequests = (CurrentAsRequestUser is null) ? new List<Request>() : RequestStore.GetAllRequests().FindAll(x => x.Username == CurrentAsRequestUser.Username);
            var FriendRequestViewDTO = new FriendRequestViewDTO();
            GetInvites(FriendRequestViewDTO, IncomingRequests, OutgoingRequests, FriendListStore, UserStore, RequestStore);
            return FriendRequestViewDTO;
        }

        public void GetInvites(FriendRequestViewDTO FriendRequestViewDTO, List<Request> RecievedInvites,
        List<Request> SentInvites, IFriendsListStore _FriendListStore,
        IUserStore _userStore, IRequestStore _RequestStore)
        {
            foreach (var Request in RecievedInvites)
            {
                var RequestUser = _RequestStore.GetRequestUserById(Request.RequestUserId);
                FriendRequestViewDTO.RecievedInvites.Add(new FriendRequestDTO()
                {
                    Id = Request.Id,
                    RequestUserId = Request.RequestUserId,
                    FriendListId = Request.FriendListId,
                    Pending = Request.Pending,
                    Username = RequestUser.Username,
                    message = $"{RequestUser.Username} would like to be your friend."
                });
            }

            foreach (var Request in SentInvites)
            {
                var InviteeFriendsList = _FriendListStore.GetFriendListById(Request.FriendListId);
                var Invitee = _userStore.GetByIdentityUserId(InviteeFriendsList.UserId);
                FriendRequestViewDTO.SentInvites.Add(new FriendRequestDTO()
                {
                    Id = Request.Id,
                    RequestUserId = Request.RequestUserId,
                    FriendListId = Request.FriendListId,
                    Pending = Request.Pending,
                    Username = Invitee.Username
                });
            }
        }


    }
}