using System;
using FruityNET.Data;
using FruityNET.Entities;
using FruityNET.IEntityStore;
using System.Linq;
using System.Collections.Generic;

namespace FruityNET.EntityStore
{
    public class GroupStore : IGroupStore
    {
        private readonly ApplicationDbContext _Context;
        public GroupStore(ApplicationDbContext _Context)
        {
            this._Context = _Context;
        }
        public Group CreateGroup(Group Group)
        {
            _Context.Group.Add(Group);
            _Context.SaveChanges();
            return Group;
        }

        public GroupOwner CreateGroupOwner(GroupOwner GroupOwner)
        {
            _Context.GroupOwner.Add(GroupOwner);
            _Context.SaveChanges();
            return GroupOwner;
        }

        public GroupUser CreateGroupUser(GroupUser GroupUser)
        {
            _Context.GroupUser.Add(GroupUser);
            _Context.SaveChanges();
            return GroupUser;
        }

        public Group DeleteGroup(Group Group)
        {
            _Context.Group.Remove(Group);
            _Context.SaveChanges();
            return Group;
        }

        public GroupOwner DeleteGroupOwner(GroupOwner groupOwner)
        {
            _Context.GroupOwner.Remove(groupOwner);
            _Context.SaveChanges();
            return groupOwner;
        }

        public GroupUser DeleteGroupUser(GroupUser groupUser)
        {
            _Context.GroupUser.Remove(groupUser);
            _Context.SaveChanges();
            return groupUser;
        }

        public List<Group> GetAllGroupsByUser(string UserId)
        {
            return _Context.Group.ToList().FindAll(x => x.UserId == UserId);
        }

        public List<Group> GetAllGroup()
        {
            return _Context.Group.ToList();
        }

        public Group GetGroupById(Guid Id)
        {
            return _Context.Group.Find(Id);
        }

        public Group GetGroupByName(string Groupname)
        {
            return _Context.Group.FirstOrDefault(x => x.Name == Groupname);
        }

        public List<GroupUser> GetGroupMembers(Guid GroupId)
        {
            var Query = from x in _Context.GroupUser
                        where x.GroupId == GroupId
                        select x;
            return Query.ToList();
        }

        public GroupOwner GetGroupOwner(Guid GroupId)
        {
            return _Context.GroupOwner.FirstOrDefault(x => x.GroupId == GroupId);
        }

        public GroupUser GetGroupMemberById(Guid Id)
        {
            return _Context.GroupUser.FirstOrDefault(x => x.Id == Id);
        }

        public List<Group> GetGroupsWithUser(string Id)
        {
            var groups = _Context.Group.ToList();
            var groupsWithUser = new List<Group>();
            foreach (var Group in groups)
            {
                var members = GetGroupMembers(Group.Id);
                var userAsMember = members.FirstOrDefault(x => x.UserId.Equals(Id));
                if (userAsMember != null)
                    groupsWithUser.Add(Group);
            }
            return groupsWithUser;

        }
    }
}