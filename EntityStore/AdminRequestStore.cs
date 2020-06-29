using System;
using System.Collections.Generic;
using System.Linq;
using FruityNET.Data;
using FruityNET.Entities;
using FruityNET.Enums;
using FruityNET.IEntityStore;

namespace FruityNET.EntityStore
{
    public class AdminRequestStore : IAdminRequestStore
    {
        private readonly ApplicationDbContext _Context;

        public AdminRequestStore(ApplicationDbContext _Context)
        {
            this._Context = _Context;
        }

        public AdminRequest AddRequest(AdminRequest adminRequest)
        {
            _Context.AdminRequest.Add(adminRequest);
            return adminRequest;
        }

        public AdminRequestor AddRequestor(AdminRequestor adminRequestor)
        {
            _Context.AdminRequestor.Add(adminRequestor);
            return adminRequestor;
        }

        public UserAccount ApproveAdmin(UserAccount user)
        {
            user.UserType = UserType.Admin;
            _Context.SaveChanges();
            return user;
        }

        public AdminRequest DeleteRequest(Guid RequestID)
        {
            var existingRequest = _Context.AdminRequest.Find(RequestID);
            _Context.AdminRequest.Remove(existingRequest);
            _Context.SaveChanges();
            return existingRequest;
        }

        public AdminRequestor DeleteRequestor(Guid RequestorID)
        {
            var existingRequestor = _Context.AdminRequestor.Find(RequestorID);
            _Context.AdminRequestor.Remove(existingRequestor);
            _Context.SaveChanges();
            return existingRequestor;
        }

        public AdminApprovalBox GetAdminBox(Guid SiteOwnerID)
        {
            return _Context.AdminApproval.FirstOrDefault(x => x.SiteOwnerId.Equals(SiteOwnerID));
        }

        public List<AdminRequest> GetAll()
        {
            return _Context.AdminRequest.ToList();
        }

        public AdminRequest GetRequestById(Guid RequestID)
        {
            var Request = _Context.AdminRequest.Find(RequestID);
            return Request;
        }

        public SiteOwner GetSiteOwner()
        {
            return _Context.SiteOwner.FirstOrDefault();
        }

        public AdminRequestor GetUserById(Guid RequestorID)
        {
            var Requestor = _Context.AdminRequestor.Find(RequestorID);
            return Requestor;
        }
    }
}