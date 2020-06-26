using System;
using System.Collections.Generic;
using FruityNET.Entities;

namespace FruityNET.IEntityStore
{
    public interface IGroupRequestStore
    {

        List<GroupRequestUser> GetAllRequestUsers();
        List<GroupRequest> GetAllGroupRequests();
        List<GroupRequest> GetGroupRequestsOfGroup(Guid GroupId);
        GroupRequestUser GetGroupRequestUser(Guid Id);
        GroupRequest GetGroupRequest(Guid Id);
        GroupRequest DeleteRequest(Guid Id);
        GroupRequestUser DeleteRequestUser(Guid Id);
        GroupRequest SendRequest(GroupRequest request);
        GroupRequestUser NewRequestUser(GroupRequestUser requestUser);
    }
}