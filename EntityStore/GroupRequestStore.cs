using System;
using System.Collections.Generic;
using FruityNET.Data;
using FruityNET.Entities;
using FruityNET.IEntityStore;
using System.Linq;

namespace FruityNET.EntityStore
{
    public class GroupRequestStore : IGroupRequestStore
    {
        private ApplicationDbContext _Context;

        public GroupRequestStore(ApplicationDbContext Context)
        {
            _Context = Context;
        }
        public GroupRequest DeleteRequest(Guid Id)
        {
            var groupRequest = _Context.GroupRequest.Find(Id);
            _Context.GroupRequest.Remove(groupRequest);
            _Context.SaveChanges();
            return groupRequest;

        }

        public GroupRequestUser DeleteRequestUser(Guid Id)
        {
            var Requestor = _Context.GroupRequestUser.Find(Id);
            _Context.GroupRequestUser.Remove(Requestor);
            _Context.SaveChanges();
            return Requestor;

        }

        public List<GroupRequest> GetAllGroupRequests()
        {
            var Query = from x in _Context.GroupRequest
                        select x;
            return Query.ToList();

        }

        public List<GroupRequestUser> GetAllRequestUsers()
        {
            var Query = from x in _Context.GroupRequestUser
                        select x;
            return Query.ToList();
        }

        public GroupRequest GetGroupRequest(Guid Id)
        {
            return _Context.GroupRequest.Find(Id);
        }

        public List<GroupRequest> GetGroupRequestsOfGroup(Guid GroupID)
        {
            var Query = from x in _Context.GroupRequest
                        where x.GroupId.Equals(GroupID)
                        select x;
            return Query.ToList();
        }

        public GroupRequestUser GetGroupRequestUser(Guid Id)
        {
            return _Context.GroupRequestUser.Find(Id);
        }

        public GroupRequest SendRequest(GroupRequest request)
        {
            _Context.GroupRequest.Add(request);
            _Context.SaveChanges();
            return request;
        }
        public GroupRequestUser NewRequestUser(GroupRequestUser requestUser)
        {
            _Context.GroupRequestUser.Add(requestUser);
            _Context.SaveChanges();
            return requestUser;
        }
    }
}