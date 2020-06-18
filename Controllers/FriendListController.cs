using System;
using System.Collections.Generic;
using FruityNET.Data;
using FruityNET.DTOs;
using FruityNET.Entities;
using FruityNET.IEntityStore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using FruityNET.Exceptions;
using FruityNET.ParameterStrings;
using Microsoft.Extensions.Logging;

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
        private readonly INotificationBox _notificationBox;

        private readonly ILogger<FriendListController> _logger;





        public FriendListController(UserManager<User> userManager, SignInManager<User> signInManager, ApplicationDbContext _context,
        IUserStore _userStore, IFriendsListStore _FriendListStore, IRequestStore _RequestStore, INotificationBox _notificationBox,
        ILogger<FriendListController> _logger)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this._context = _context;
            this._userStore = _userStore;
            this._FriendListStore = _FriendListStore;
            this._RequestStore = _RequestStore;
            this._notificationBox = _notificationBox;
            this._logger = _logger;
        }


        [HttpGet]
        public IActionResult Index()
        {

            return View();
        }


        [HttpGet]
        public IActionResult SendFriendInvite(Guid Id)
        {
            try
            {
                var CurrentUser = _context.Users.Find(userManager.GetUserId(User));
                if (CurrentUser is null)
                    throw new DomainException(ErrorMessages.NotSignedIn);

                var FriendList = _FriendListStore.GetFriendListById(Id);
                if (FriendList is null)
                    throw new DomainException();
                return View(FriendList);

            }
            catch (DomainException ex)
            {
                _logger.LogError(ex.Message);
                if (ex.Message.Equals(ErrorMessages.NotSignedIn))
                    return RedirectToAction(ActionName.Login, ControllerName.Accounts);

                return RedirectToAction(ActionName.NotFound, ControllerName.Accounts);
            }



        }


        [HttpPost]
        public IActionResult SendFriendInvite(FriendList friendList)
        {
            try
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
                ViewBag.Message = "Success";
                return RedirectToAction(ActionName.Search, ControllerName.Accounts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return RedirectToAction(ActionName.ServerError, ControllerName.Accounts);
            }
        }

        [HttpGet]
        public IActionResult FriendRequests()
        {
            try
            {
                var _currentUser = _context.Users.Find(userManager.GetUserId(User));
                if (_currentUser is null)
                    throw new DomainException(ErrorMessages.NotSignedIn);

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
                        Username = Invitee.Username
                    });
                }
                return View(friendRequestViewDTO);
            }
            catch (DomainException ex)
            {
                _logger.LogError(ex.Message);
                return RedirectToAction(ActionName.Login, ControllerName.Accounts);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return RedirectToAction(ActionName.ServerError, ControllerName.Accounts);


            }



        }

        [HttpGet]
        public IActionResult AcceptRequest(Guid Id)
        {

            try
            {
                var currentUser = _context.Users.Find(userManager.GetUserId(User));
                if (currentUser is null)
                    throw new DomainException(ErrorMessages.NotSignedIn);


                return View(new RequestDTO() { RequestID = Id });
            }
            catch (DomainException ex)
            {
                _logger.LogError(ex.Message);
                return RedirectToAction("Login", "Accounts");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return RedirectToAction(ActionName.ServerError, ControllerName.Accounts);
            }


        }


        [HttpPost]
        public IActionResult ConfirmRequest(Guid Id)
        {
            try
            {
                var currentUser = _context.Users.Find(userManager.GetUserId(User));
                var request = _RequestStore.GetRequestById(Id);
                if (request is null)
                    return RedirectToAction("NotFound", "Accounts");

                var requestUser = _RequestStore.GetRequestUserById(request.RequestUserId);
                var currentUserFriendList = _FriendListStore.GetFriendListOfUser(currentUser.Id);
                var requestUserAccount = _userStore.GetByUsername(requestUser.Username);
                var RequestorFriendList = _FriendListStore.GetFriendListOfUser(requestUserAccount.UserId);
                var CurrentUserNotificationBox = _notificationBox.GetNotificationBoxByUserId(currentUser.Id);
                var OtherUserNotificationBox = _notificationBox.GetNotificationBoxByUserId(requestUser.UserId);

                var FriendForCurrentUser = new FriendUser()
                {
                    UserId = requestUserAccount.UserId,
                    Username = requestUserAccount.Username,
                    FriendListId = currentUserFriendList.Id
                };
                var FriendForOtherUser = new FriendUser()
                {
                    UserId = currentUser.Id,
                    Username = currentUser.UserName,
                    FriendListId = RequestorFriendList.Id
                };


                SendNotifcations(CurrentUserNotificationBox.Id, OtherUserNotificationBox.Id,
                FriendForCurrentUser.Username, FriendForOtherUser.Username);
                CreateNewFriends(FriendForCurrentUser, FriendForOtherUser);

                _RequestStore.DeleteRequest(request);
                _RequestStore.DeleteRequestUser(requestUser);
                _context.SaveChanges();
                return RedirectToAction("FriendRequests", "FriendList");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return RedirectToAction(ActionName.ServerError, ControllerName.Accounts);
            }


        }

        [HttpGet]
        public IActionResult DeclineRequest(Guid Id)
        {
            var request = _RequestStore.GetRequestById(Id);
            if (request is null)
                return RedirectToAction("NotFound", "Accounts");
            var requestUser = _RequestStore.GetRequestUserById(request.RequestUserId);

            var existingUser = _userStore.GetByIdentityUserId(requestUser.UserId);
            var CurrentUserAccount = _userStore.GetByIdentityUserId(_context.Users.Find(userManager.GetUserId(User)).Id);
            if (CurrentUserAccount is null)
                return RedirectToAction("Login", "Accounts");
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
            if (_currentUser is null)
                return RedirectToAction("Login", "Accounts");
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

        private protected void SendNotifcations(Guid CurrentUserNotificationBoxID, Guid OtherUserNotificationBoxID,
                string FriendForCurrentUserUsername, string FriendForOtherUserUsername)
        {
            _notificationBox.SendNotifcation(new Notification()
            {
                Message = $"You and {FriendForCurrentUserUsername} are now friends.",
                NotificationBoxId = CurrentUserNotificationBoxID,
                RecieverUsername = FriendForOtherUserUsername
            });
            _notificationBox.SendNotifcation(new Notification()
            {
                Message = $"You and {FriendForOtherUserUsername} are now friends.",
                NotificationBoxId = OtherUserNotificationBoxID,
                RecieverUsername = FriendForCurrentUserUsername
            });
        }

        private protected void CreateNewFriends(FriendUser FriendForCurrentUser, FriendUser FriendForOtherUser)
        {
            _FriendListStore.AddFriend(FriendForCurrentUser);
            _FriendListStore.AddFriend(FriendForOtherUser);
        }


    }
}