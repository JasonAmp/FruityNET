using System;
using System.Collections.Generic;
using System.Linq;
using FruityNET.Data;
using FruityNET.Entities;
using FruityNET.IEntityStore;
namespace FruityNET.EntityStore
{
    public class FriendListStore : IFriendsListStore
    {
        private ApplicationDbContext _Context;
        public FriendListStore(ApplicationDbContext _Context)
        {
            this._Context = _Context;
        }

        public FriendList CreateFriendList(FriendList FriendList)
        {
            _Context.FriendList.Add(FriendList);
            _Context.SaveChanges();
            return FriendList;
        }
        public FriendUser AddFriend(FriendUser Friend)
        {
            _Context.FriendUser.Add(Friend);
            _Context.SaveChanges();
            return Friend;
        }

        public FriendList GetFriendListOfUser(string UserId)
        {
            return _Context.FriendList.FirstOrDefault(x => x.UserId == UserId);
        }

        public List<Request> GetIncomingFriendRequests(Guid FriendListId)
        {

            var query = from x in _Context.Request
                        where (x.FriendListId == FriendListId)
                        select x;

            return query.ToList();

        }

        public FriendUser Unfriend(Guid FriendId)
        {
            var existingFriendUser = _Context.FriendUser.Find(FriendId);
            _Context.FriendUser.Remove(existingFriendUser);
            _Context.SaveChanges();
            return existingFriendUser;
        }

        public FriendList GetFriendListById(Guid Id)
        {
            var FriendList = _Context.FriendList.Find(Id);
            return FriendList;
        }

        public List<FriendUser> GetFriendsOfUser(Guid FriendListId)
        {
            var query = from x in _Context.FriendUser
                        where (x.FriendListId == FriendListId)
                        select x;

            return query.ToList();
        }

        public bool IsFriendsOfUser(Guid FriendListId, string UserId)
        {
            var query = from x in _Context.FriendUser
                        where (x.FriendListId == FriendListId)
                        select x;

            var existingFriend = query.FirstOrDefault(x => x.UserId == UserId);
            if (existingFriend != null)
                return true;

            return false;
        }

        public FriendUser GetFriendById(Guid Id)
        {
            var existingFriend = _Context.FriendUser.Find(Id);
            return existingFriend;
        }
    }
}