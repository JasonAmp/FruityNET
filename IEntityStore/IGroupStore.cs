using System;
using System.Collections.Generic;
using FruityNET.Entities;

namespace FruityNET.IEntityStore
{
    public interface IGroupStore
    {
        Group CreateGroup(Group Group);
        GroupUser CreateGroupUser(GroupUser GroupUser);
        GroupOwner CreateGroupOwner(GroupOwner GroupOwner);
        Group DeleteGroup(Group Group);
        Group GetGroupById(Guid Id);
        List<Group> GetAllGroupsByUser(string UserId);
        GroupUser DeleteGroupUser(GroupUser groupUser);
        GroupOwner DeleteGroupOwner(GroupOwner groupOwner);
        List<GroupUser> GetGroupMembers(Guid GroupId);
        Group GetGroupByName(string Groupname);

        GroupOwner GetGroupOwner(Guid GroupId);
        GroupUser GetGroupMemberById(Guid Id);
    }

}