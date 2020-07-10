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
using FruityNET.Queries;

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
        public IActionResult SendInvite(Guid Id)
        {
            try
            {
                var CurrentUser = _context.Users.Find(userManager.GetUserId(User));
                if (CurrentUser is null)
                    throw new DomainException(ErrorMessages.NotSignedIn);

                var FriendList = _FriendListStore.GetFriendListById(Id);
                if (FriendList is null)
                    throw new DomainException();


                var CurrentFriendList = _FriendListStore.GetFriendListOfUser(CurrentUser.Id);
                var RequestsOfCurrent = _FriendListStore.GetIncomingFriendRequests(CurrentFriendList.Id);
                var Recipient = _userStore.GetByIdentityUserId(FriendList.UserId);
                var RequestsOfRecipient = _FriendListStore.GetIncomingFriendRequests(FriendList.Id);
                var CurrentAsSender = RequestsOfRecipient.FirstOrDefault(x => x.Username.Equals(CurrentUser.UserName));
                var PendingFromOther = RequestsOfCurrent.FirstOrDefault(x => x.Username.Equals(Recipient.Username));


                if (CurrentAsSender != null || PendingFromOther != null)
                    throw new DomainException(ErrorMessages.PendingRequest);

                return View(new SendRequestDTO()
                {
                    RecipientFriendListID = FriendList.Id,
                    RecipientUsername = Recipient.Username,
                    RecipientAccountID = Recipient.Id,

                });

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
        public IActionResult ConfirmInvite(Guid Id)
        {
            var SendRequestDTO = new SendRequestDTO()
            { };
            try
            {
                var _currentUser = _context.Users.Find(userManager.GetUserId(User));
                var existingFriendList = _FriendListStore.GetFriendListById(Id);
                var Requests = _FriendListStore.GetIncomingFriendRequests(existingFriendList.Id);
                var Recipient = _userStore.GetByIdentityUserId(existingFriendList.UserId);
                var PendingRequest = Requests.FirstOrDefault(x => x.Username.Equals(_currentUser.UserName));
                if (PendingRequest != null)
                    throw new DomainException(ErrorMessages.PendingRequest);

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

                _RequestStore.SendRequest(Request);

                _context.SaveChanges();
                ViewBag.Message = "Success";
                SendRequestDTO.RecipientFriendListID = Id;
                SendRequestDTO.RecipientUsername = Recipient.Username;
                SendRequestDTO.RecipientAccountID = Recipient.Id;

                return View(SendRequestDTO);
            }
            catch (DomainException ex)
            {
                _logger.LogError(ex.Message);
                ViewBag.Message = "Error";
                return View(SendRequestDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return RedirectToAction(ActionName.ServerError, ControllerName.Accounts);
            }
        }

        [HttpGet]
        public IActionResult Invites()
        {
            try
            {
                var currentUser = _context.Users.Find(userManager.GetUserId(User));
                if (currentUser is null)
                    throw new DomainException(ErrorMessages.NotSignedIn);

                var existingAccount = _userStore.GetByIdentityUserId(currentUser.Id);
                var GetAllInvitesQuery = new GetAllInvitesQuery(_RequestStore, _userStore, currentUser, existingAccount, _FriendListStore);
                return View(GetAllInvitesQuery.Handle());
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
                return RedirectToAction("Invites", "FriendList");
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
        public IActionResult Unfriend(Guid Id)
        {
            try
            {
                var _currentUser = _context.Users.Find(userManager.GetUserId(User));
                if (_currentUser is null)
                    throw new DomainException(ErrorMessages.NotSignedIn);
                var existingFriend = _FriendListStore.GetFriendById(Id);
                if (existingFriend is null)
                    throw new DomainException(ErrorMessages.UserDoesNotExist);

                return View(existingFriend);
            }
            catch (DomainException ex)
            {
                _logger.LogError(ex.Message);
                if (ex.Message.Equals(ErrorMessages.NotSignedIn))
                    return RedirectToAction("Login", "Accounts");

                return RedirectToAction("NotFound", "Accounts");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return RedirectToAction(ActionName.ServerError, ControllerName.Accounts);
            }



        }


        [HttpPost]
        public IActionResult Unfriend(FriendUser Friend)
        {
            try
            {
                var _currentUser = _context.Users.Find(userManager.GetUserId(User));

                var FriendsListOfFriend = _FriendListStore.GetFriendListOfUser(_FriendListStore.GetFriendById(Friend.Id).UserId);
                var FriendUser = _FriendListStore.GetFriendById(Friend.Id);
                var CurrentUserAsFriendUser = _FriendListStore.GetFriendsOfUser(FriendsListOfFriend.Id)
                .FirstOrDefault(x => x.UserId == _currentUser.Id);

                _userStore.GetByIdentityUserId(_context.Users.Find(userManager.GetUserId(User)).Id).FriendList.Remove(FriendUser);
                _userStore.GetByIdentityUserId(FriendUser.UserId).FriendList.Remove(CurrentUserAsFriendUser);

                _FriendListStore.Unfriend(FriendUser.Id);
                _FriendListStore.Unfriend(CurrentUserAsFriendUser.Id);
                return RedirectToAction("Profile", "Accounts");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return RedirectToAction(ActionName.ServerError, ControllerName.Accounts);
            }



        }

        private protected void SendNotifcations(Guid CurrentUserNotificationBoxID, Guid OtherUserNotificationBoxID,
                string FriendForCurrentUserUsername, string FriendForOtherUserUsername)
        {
            _notificationBox.SendNotifcation(new Notification()
            {
                Message = $"You and {FriendForCurrentUserUsername} are now friends.",
                NotificationBoxId = CurrentUserNotificationBoxID,
                RecieverUsername = FriendForOtherUserUsername,
                NotificationDate = DateTime.Now

            });
            _notificationBox.SendNotifcation(new Notification()
            {
                Message = $"You and {FriendForOtherUserUsername} are now friends.",
                NotificationBoxId = OtherUserNotificationBoxID,
                RecieverUsername = FriendForCurrentUserUsername,
                NotificationDate = DateTime.Now

            });
        }

        private protected void CreateNewFriends(FriendUser FriendForCurrentUser, FriendUser FriendForOtherUser)
        {
            _FriendListStore.AddFriend(FriendForCurrentUser);
            _FriendListStore.AddFriend(FriendForOtherUser);
        }

        [HttpGet]
        public IActionResult CancelInvite(Guid Id)
        {
            try
            {
                var _currentUser = _context.Users.Find(userManager.GetUserId(User));
                if (_currentUser is null)
                    throw new DomainException(ErrorMessages.NotSignedIn);

                var existingRequest = _RequestStore.GetRequestById(Id);
                if (existingRequest is null)
                    throw new DomainException(ErrorMessages.RequestDoesNotExist);

                var recipientFriends = _FriendListStore.GetFriendListById(existingRequest.FriendListId);

                var recipientUser = _userStore.GetByIdentityUserId(recipientFriends.UserId);

                var existingRequestUser = _RequestStore.GetRequestUserById(existingRequest.RequestUserId);
                if (!existingRequestUser.Username.Equals(_currentUser.UserName))
                    throw new ForbiddenException(ErrorMessages.ForbiddenAccess);

                var existingAccount = _userStore.GetByIdentityUserId(existingRequestUser.UserId);

                var request = new CancelRequestDTO()
                {
                    RequestID = existingRequest.Id,
                    RequestUserID = existingRequestUser.Id,
                    RequestUsername = existingRequestUser.Username,
                    AccountID = existingAccount.Id,
                    RecipientAccountID = recipientUser.Id,
                    RecipientUsername = recipientUser.Username
                };
                return View(request);

            }
            catch (DomainException ex)
            {
                _logger.LogError(ex.Message);
                return RedirectToAction("NotFound", "Accounts");
            }
            catch (ForbiddenException ex)
            {
                _logger.LogError(ex.Message);
                return RedirectToAction(ActionName.NotAuthorized);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return RedirectToAction(ActionName.ServerError, ControllerName.Accounts);
            }
        }

        [HttpPost]
        public IActionResult ConfirmCancel(Guid Id)
        {
            try
            {
                var existingUser = _RequestStore.GetRequestUserById(Id);
                if (existingUser is null)
                    throw new DomainException(ErrorMessages.UserDoesNotExist);
                var existingAccount = _userStore.GetByIdentityUserId(existingUser.UserId);
                var existingRequest = _RequestStore.GetAllRequests()
                .FirstOrDefault(x => x.RequestUserId.Equals(Id));
                var RecipientFriendsList = _FriendListStore.GetFriendListById(existingRequest.FriendListId);
                var RecipientAccount = _userStore.GetByIdentityUserId(RecipientFriendsList.UserId);

                var CancelRequestDTO = new CancelRequestDTO()
                {
                    RequestUsername = new string(existingAccount.Username),
                    AccountID = new Guid(existingAccount.Id.ToString()),
                    RecipientAccountID = new Guid(RecipientAccount.Id.ToString()),
                    RecipientUsername = new string(RecipientAccount.Username)
                };

                _RequestStore.DeleteRequestUser(existingUser);
                _context.SaveChanges();
                ViewBag.Message = "Success";
                return View(CancelRequestDTO);

            }
            catch (DomainException ex)
            {
                _logger.LogError(ex.Message);
                return RedirectToAction("NotFound", "Accounts");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return RedirectToAction(ActionName.ServerError, ControllerName.Accounts);
            }
        }




    }
}