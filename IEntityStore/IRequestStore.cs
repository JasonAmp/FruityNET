using System;
using System.Collections.Generic;
using FruityNET.Entities;

namespace FruityNET.IEntityStore
{
    public interface IRequestStore
    {
        Request SendRequest(Request Request);
        RequestUser CreateRequestUser(RequestUser RequestUser);
        Request DeleteRequest(Request Request);
        RequestUser DeleteRequestUser(RequestUser RequestUser);
        List<RequestUser> GetAllRequestUsers();
        RequestUser GetRequestUserById(Guid Id);
        List<Request> GetAllRequests();
        Request GetRequestById(Guid Id);
        List<Request> PendingRequest(Guid Id);


    }



}