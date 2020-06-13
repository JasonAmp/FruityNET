using System;
using System.Collections.Generic;
using FruityNET.Entities;

namespace FruityNET.IEntityStore
{
    public interface IFriendsListStore
    {
        FriendList GetFriendListOfUser(string UserId);
        List<FriendUser> GetFriendsOfUser(Guid FriendListId);
        FriendList GetFriendListById(Guid Id);
        List<Request> GetIncomingFriendRequests(Guid FriendListId);
        FriendUser AddFriend(FriendUser Friend);
        FriendUser Unfriend(Guid FriendId);
        FriendList CreateFriendList(FriendList FriendList);
        FriendUser GetFriendById(Guid Id);
        bool IsFriendsOfUser(Guid FriendListId, string UserId);

    }
}