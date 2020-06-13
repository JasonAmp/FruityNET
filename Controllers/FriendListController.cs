using System;
using System.Collections.Generic;
using FruityNET.Data;
using FruityNET.DTOs;
using FruityNET.Entities;
using FruityNET.IEntityStore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace FruityNET.Controllers
{
    public class FriendListController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        private readonly ApplicationDbContext _context;
        private readonly IUserStore _userStore;
        private readonly IFriendsListStore _FriendListStore;
        private readonly IRequestStore _RequestStore;

        private readonly AcceptedRequestDTO _acceptedRequest;

        private readonly INotificationBox _notificationBox;




        public FriendListController(UserManager<User> userManager, SignInManager<User> signInManager, ApplicationDbContext _context,
        IUserStore _userStore, IFriendsListStore _FriendListStore, IRequestStore _RequestStore, INotificationBox _notificationBox)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this._context = _context;
            this._userStore = _userStore;
            this._FriendListStore = _FriendListStore;
            this._RequestStore = _RequestStore;
            this._notificationBox = _notificationBox;
        }


        [HttpGet]
        public IActionResult Index()
        {

            return View();
        }


        [HttpGet]
        public IActionResult SendFriendInvite(Guid Id)
        {
            var FriendList = _FriendListStore.GetFriendListById(Id);

            return View(FriendList);
        }


        [HttpPost]
        public IActionResult SendFriendInvite(FriendList friendList)
        {
            var _currentUser = _context.Users.Find(userManager.GetUserId(User));
            var existingFriendList = _FriendListStore.GetFriendListById(friendList.Id);

            var RequestUser = new RequestUser()
            {
                UserId = _currentUser.Id,
                Username = _currentUser.UserName,
            };
            _RequestStore.CreateRequestUser(RequestUser);
            var Request = new Request()
            {
                Pending = true,
                FriendListId = existingFriendList.Id,
                RequestUserId = RequestUser.Id,
                Username = _currentUser.UserName
            };
            var Notification = new Notification()
            {
                Message = $"You have a Friend Invite from {RequestUser.Username}",
                NotificationBoxId = _notificationBox.GetNotificationBoxByUserId(existingFriendList.UserId).Id,
                RecieverUsername = _userStore.GetByIdentityUserId(existingFriendList.UserId).Username
            };
            _RequestStore.SendRequest(Request);
            _notificationBox.SendNotifcation(Notification);

            _context.SaveChanges();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult FriendRequests()
        {
            var _currentUser = _context.Users.Find(userManager.GetUserId(User));
            var existingAccount = _userStore.GetByIdentityUserId(_currentUser.Id);

            var FriendList = _FriendListStore.GetFriendListOfUser(existingAccount.UserId);
            var IncomingRequests = _RequestStore.GetAllRequests().FindAll(x => x.FriendListId == FriendList.Id);
            var CurrentAsRequestUser = _RequestStore.GetAllRequestUsers().FirstOrDefault(x => x.UserId == _currentUser.Id);
            var OutgoingRequests = (CurrentAsRequestUser is null) ? new List<Request>() : _RequestStore.GetAllRequests().FindAll(x => x.Username == CurrentAsRequestUser.Username);


            var friendRequestViewDTO = new FriendRequestViewDTO();

            foreach (var Request in IncomingRequests)
            {
                var RequestUser = _RequestStore.GetRequestUserById(Request.RequestUserId);
                friendRequestViewDTO.IncomingRequests.Add(new FriendRequestDTO()
                {
                    Id = Request.Id,
                    RequestUserId = Request.RequestUserId,
                    FriendListId = Request.FriendListId,
                    Pending = Request.Pending,
                    UserResponse = Request.UserResponse,
                    Username = RequestUser.Username
                });
            }

            foreach (var Request in OutgoingRequests)
            {

                var InviteeFriendsList = _FriendListStore.GetFriendListById(Request.FriendListId);
                var Invitee = _userStore.GetByIdentityUserId(InviteeFriendsList.UserId);
                friendRequestViewDTO.OutgoingRequests.Add(new FriendRequestDTO()
                {
                    Id = Request.Id,
                    RequestUserId = Request.RequestUserId,
                    FriendListId = Request.FriendListId,
                    Pending = Request.Pending,
                    UserResponse = Request.UserResponse,
                    Username = Invitee.Username
                });
            }
            return View(friendRequestViewDTO);

        }

        [HttpGet]
        public IActionResult AcceptRequest(Guid Id)
        {
            var _currentUser = _context.Users.Find(userManager.GetUserId(User));
            var FriendList = _FriendListStore.GetFriendListOfUser(_currentUser.Id);
            var request = _RequestStore.GetRequestById(Id);
            var requestUser = _RequestStore.GetRequestUserById(request.RequestUserId);
            var existingUser = _userStore.GetByUsername(requestUser.Username);

            var FriendForCurrentUser = new FriendUser()
            {
                UserId = requestUser.UserId,
                Username = requestUser.Username,
                FriendListId = FriendList.Id
            };
            var FriendForOtherUser = new FriendUser()
            {
                UserId = _currentUser.Id,
                Username = _currentUser.UserName,
                FriendListId = _FriendListStore.GetFriendListOfUser(existingUser.UserId).Id
            };
            _FriendListStore.AddFriend(FriendForCurrentUser);
            _FriendListStore.AddFriend(FriendForOtherUser);
            FriendList.Requests.Remove(request);
            _RequestStore.DeleteRequest(request);
            _RequestStore.DeleteRequestUser(requestUser);

            var Notification = new Notification()
            {
                Message = $"You and {existingUser.Username} are now friends.",
                NotificationBoxId = _notificationBox.GetNotificationBoxByUserId(_currentUser.Id).Id,
                RecieverUsername = _userStore.GetByIdentityUserId(_currentUser.Id).Username
            };

            var NotificationForFriend = new Notification()
            {
                Message = $"You and {_currentUser.UserName} are now friends.",
                NotificationBoxId = _notificationBox.GetNotificationBoxByUserId(_currentUser.Id).Id,
                RecieverUsername = _userStore.GetByIdentityUserId(existingUser.UserId).Username
            };
            _notificationBox.SendNotifcation(Notification);
            _notificationBox.SendNotifcation(NotificationForFriend);
            _context.SaveChanges();
            return View(FriendForCurrentUser);
        }


        [HttpPost]
        public IActionResult AcceptRequest(FriendUser friend)
        {

            return RedirectToAction("FriendRequests", "FriendList");

        }

        [HttpGet]
        public IActionResult DeclineRequest(Guid Id)
        {
            var request = _RequestStore.GetRequestById(Id);
            var requestUser = _RequestStore.GetRequestUserById(request.RequestUserId);
            var existingUser = _userStore.GetByIdentityUserId(requestUser.UserId);
            var CurrentUserAccount = _userStore.GetByIdentityUserId(_context.Users.Find(userManager.GetUserId(User)).Id);
            _RequestStore.DeleteRequest(request);
            _RequestStore.DeleteRequestUser(requestUser);
            CurrentUserAccount.IncomingRequests.Remove(request);
            existingUser.OutgoingRequests.Remove(request);
            return View(Request);
        }


        [HttpPost]
        public IActionResult DeclineRequest()
        {

            return RedirectToAction("Profile", "Accounts");

        }


        [HttpGet]
        public IActionResult UnfriendUser(Guid Id)
        {
            var _currentUser = _context.Users.Find(userManager.GetUserId(User));
            var FriendsListOfFriend = _FriendListStore.GetFriendListOfUser(_FriendListStore.GetFriendById(Id).UserId);
            var FriendUser = _FriendListStore.GetFriendById(Id);
            var CurrentUserAsFriendUser = _FriendListStore.GetFriendsOfUser(FriendsListOfFriend.Id)
            .FirstOrDefault(x => x.UserId == _currentUser.Id);

            _userStore.GetByIdentityUserId(_context.Users.Find(userManager.GetUserId(User)).Id).FriendList.Remove(FriendUser);
            _userStore.GetByIdentityUserId(FriendUser.UserId).FriendList.Remove(CurrentUserAsFriendUser);

            _FriendListStore.Unfriend(FriendUser.Id);
            _FriendListStore.Unfriend(CurrentUserAsFriendUser.Id);

            return View(FriendUser);
        }


        [HttpPost]
        public IActionResult UnfriendUser()
        {
            return RedirectToAction("Profile", "Accounts");

        }


    }
}