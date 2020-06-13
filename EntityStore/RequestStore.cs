using System;
using System.Collections.Generic;
using FruityNET.Data;
using FruityNET.Entities;
using FruityNET.IEntityStore;
using System.Linq;

namespace FruityNET.EntityStore
{
    public class RequestStore : IRequestStore
    {
        private readonly ApplicationDbContext _context;
        public RequestStore(ApplicationDbContext _context)
        {
            this._context = _context;
        }
        public RequestUser CreateRequestUser(RequestUser RequestUser)
        {
            _context.RequestUser.Add(RequestUser);
            _context.SaveChanges();
            return RequestUser;
        }

        public Request GetRequestById(Guid Id)
        {
            return _context.Request.Find(Id);
        }


        public List<RequestUser> GetAllRequestUsers()
        {
            return _context.RequestUser.ToList();
        }

        public List<Request> GetAllRequests()
        {
            return _context.Request.ToList();
        }



        public Request DeleteRequest(Request Request)
        {
            _context.Request.Remove(Request);
            _context.SaveChanges();
            return Request;
        }



        public Request SendRequest(Request Request)
        {
            _context.Request.Add(Request);
            _context.SaveChanges();
            return Request;
        }

        public RequestUser GetRequestUserById(Guid Id)
        {
            return _context.RequestUser.Find(Id);

        }

        public RequestUser GetRequestUserByUserId(string Id)
        {
            return _context.RequestUser.FirstOrDefault(x => x.UserId == Id);
        }

        public RequestUser DeleteRequestUser(RequestUser RequestUser)
        {
            _context.RequestUser.Remove(RequestUser);
            _context.SaveChanges();
            return RequestUser;
        }

        public Request GetRequestByUserId(Guid Id)
        {
            return _context.Request.FirstOrDefault(x => x.RequestUserId == Id);

        }



        public RequestUser GetRequestUserByUsername(string Username)
        {
            return _context.RequestUser.FirstOrDefault(x => x.Username == Username);
        }



        public List<Request> GetRequestsOfFriendList(Guid Id)
        {
            var query = from x in _context.Request
                        where (x.FriendListId == Id)
                        select x;
            return query.ToList();
        }


        public List<RequestUser> GetRequestUsersOfUser(string Username)
        {
            var query = from x in _context.RequestUser
                        where (x.Username == Username)
                        select x;
            return query.ToList();
        }

        public List<Request> PendingRequest(Guid Id)
        {

            var query = from x in _context.Request
                        where (x.FriendListId == Id)
                        select x;
            return query.ToList();
        }
    }
}