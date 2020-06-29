using System;
using System.Collections.Generic;
using FruityNET.Entities;

namespace FruityNET.IEntityStore
{
    public interface IAdminRequestStore
    {
        List<AdminRequest> GetAll();
        AdminRequest GetRequestById(Guid RequestID);

        AdminApprovalBox GetAdminBox(Guid SiteOwnerID);

        AdminRequestor GetUserById(Guid RequestorID);
        UserAccount ApproveAdmin(UserAccount user);
        AdminRequest DeleteRequest(Guid RequestID);
        AdminRequestor DeleteRequestor(Guid RequestorID);

        AdminRequest AddRequest(AdminRequest adminRequest);
        AdminRequestor AddRequestor(AdminRequestor adminRequestor);

        SiteOwner GetSiteOwner();
    }
}