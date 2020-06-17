using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FruityNET.Data;
using FruityNET.DTOs;
using FruityNET.Entities;
using FruityNET.Enums;
using FruityNET.Exceptions;
using FruityNET.IEntityStore;
using FruityNET.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using FruityNET.ParameterStrings;

namespace FruityNET.Controllers
{
    public class AccountsController : Controller
    {

        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        private readonly ApplicationDbContext _context;
        private readonly IUserStore _userStore;
        private readonly IFriendsListStore _FriendListStore;
        private readonly IRequestStore _RequestStore;
        private readonly INotificationBox _notificationBox;
        private readonly IGroupStore GroupStore;
        private readonly ILogger<AccountsController> _logger;

        public AccountsController(UserManager<User> userManager, SignInManager<User> signInManager, ApplicationDbContext _context,
        IUserStore _userStore, IFriendsListStore _FriendListStore, IRequestStore _RequestStore, INotificationBox _notificationBox,
        IGroupStore GroupStore, ILogger<AccountsController> _logger)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this._context = _context;
            this._userStore = _userStore;
            this._FriendListStore = _FriendListStore;
            this._RequestStore = _RequestStore;
            this._notificationBox = _notificationBox;
            this.GroupStore = GroupStore;
            this._logger = _logger;
        }

        [HttpGet]
        public IActionResult AdminPortal()
        {
            try
            {
                var CurrentUser = _context.Users.Find(userManager.GetUserId(User));
                if (CurrentUser is null)
                    throw new DomainException(ErrorMessages.NotSignedIn);

                var existingAccount = _userStore.GetByIdentityUserId(CurrentUser.Id);

                if (existingAccount.UserType != UserType.Admin && existingAccount.UserType != UserType.SiteOwner)
                    throw new ForbiddenException(ErrorMessages.ForbiddenAccess);


                var AdminPortalDTO = new AdminPortalViewDTO()
                {
                    UserId = CurrentUser.Id,
                    Accounts = new List<AccountDTO>()
                };
                var AllAccounts = _userStore.GetAccounts().FindAll(x => x.UserId != CurrentUser.Id);
                foreach (var user in AllAccounts)
                {
                    var AccountDTO = new AccountDTO()
                    {
                        Id = user.Id,
                        UserId = user.UserId,
                        Username = user.Username,
                        UserType = user.UserType,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        LastActive = user.LastActive,
                        DateJoined = user.DateJoined,
                        Email = user.Email
                    };
                    AdminPortalDTO.Accounts.Add(AccountDTO);
                }
                return View(AdminPortalDTO);
            }

            catch (ForbiddenException ex)
            {
                _logger.LogError(ex.Message);
                return RedirectToAction("NotAuthorized");
            }
            catch (DomainException ex)
            {
                _logger.LogError(ex.Message);
                return RedirectToAction("Login", "Accounts");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return RedirectToAction("ServerError");
            }



        }

        [HttpGet]
        public IActionResult NotAuthorized()
        {

            return View();

        }

        public IActionResult Login()
        {
            if (signInManager.IsSignedIn(User) is false)
            {
                return View();

            }
            return RedirectToAction("Index", "Home");

        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    var result = await signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, false);

                    if (result.Succeeded)
                    {
                        var existingAccount = _userStore.GetByUsername(model.UserName);
                        existingAccount.LastActive = DateTime.Now;
                        _context.SaveChanges();
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        throw new DomainException(ErrorMessages.InvalidLogin);
                    }
                }
                catch (DomainException ex)
                {
                    _logger.LogError(ex.Message);
                    ModelState.AddModelError("Error", ex.Message);

                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                    return RedirectToAction("ServerError");
                }

            }
            ModelState.AddModelError("Error", ErrorMessages.InvalidLogin);
            return View(model);

        }





        public IActionResult Register()
        {

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {

            if (ModelState.IsValid)
            {

                try
                {
                    var user = new User
                    {
                        UserName = model.UserName,
                        Email = model.Email,
                        UserType = UserType.User
                    };

                    var result = await userManager.CreateAsync(user, model.Password);
                    var userAccount = new UserAccount
                    {
                        Username = model.UserName,
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Email = model.Email,
                        UserType = UserType.User,
                        UserId = user.Id.ToString(),
                        DateJoined = DateTime.Now,
                        LastActive = DateTime.Now,
                        FriendList = new List<FriendUser>(),
                        IncomingRequests = new List<Request>(),
                        OutgoingRequests = new List<Request>(),
                    };

                    var UserFriendList = _FriendListStore.CreateFriendList(new FriendList() { UserId = user.Id });
                    var NotificationBox = _notificationBox.CreateNotificationBox(new NotificationBox() { UserId = user.Id });
                    userAccount.FriendList = UserFriendList.Users;
                    CreateAccount(userAccount);


                    if (result.Succeeded)
                    {
                        await signInManager.SignInAsync(user, isPersistent: false);
                        return RedirectToAction("Index", "Home");
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                    return RedirectToAction("ServerError");
                }
            }
            return View(model);
        }

        public UserAccount CreateAccount(UserAccount userAccount)
        {
            _userStore.CreateAccount(userAccount);
            return userAccount;

        }


        public IActionResult SignOut()
        {
            try
            {
                signInManager.SignOutAsync();
                return RedirectToAction("Login", "Accounts");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return RedirectToAction("ServerError");
            }
        }



        [HttpGet]
        public IActionResult Profile()
        {
            try
            {
                var _currentUser = _context.Users.Find(userManager.GetUserId(User));
                if (_currentUser is null)
                    throw new DomainException(ErrorMessages.NotSignedIn);

                var existingAccount = _userStore.GetByIdentityUserId(_currentUser.Id);
                var FriendList = _FriendListStore.GetFriendListOfUser(_currentUser.Id);
                var FriendUsers = _FriendListStore.GetFriendsOfUser(FriendList.Id);

                var ProfileViewModel = new ProfileViewModel()
                {
                    FirstName = existingAccount.FirstName,
                    LastName = existingAccount.LastName,
                    Location = existingAccount.Location,
                    Email = existingAccount.Email,
                    Username = existingAccount.Username,
                    Occupation = existingAccount.Occupation,
                    LastActive = existingAccount.LastActive,
                    JoinDate = existingAccount.DateJoined,
                    UserType = existingAccount.UserType,
                    Groups = GroupStore.GetAllGroupsByUser(existingAccount.UserId)
                };
                foreach (var friend in FriendUsers)
                {
                    var account = _userStore.GetByUsername(friend.Username);
                    var FriendDTO = new FriendDTO()
                    {
                        Id = friend.Id,
                        UserId = friend.UserId,
                        Username = friend.Username,
                        AccountId = account.Id
                    };
                    ProfileViewModel.Friends.Add(FriendDTO);
                }
                return View(ProfileViewModel);
            }
            catch (DomainException ex)
            {
                _logger.LogError(ex.Message);
                return RedirectToAction("Login", "Accounts");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return RedirectToAction("ServerError");
            }

        }

        public IActionResult Edit()
        {
            try
            {
                var _currentUser = _context.Users.Find(userManager.GetUserId(User));
                if (_currentUser is null)
                    throw new DomainException(ErrorMessages.NotSignedIn);

                var existingAccount = _userStore.GetByIdentityUserId(_currentUser.Id);

                return View(new EditProfileViewModel
                {
                    UserId = _currentUser.Id,
                    FirstName = existingAccount.FirstName,
                    LastName = existingAccount.LastName,
                    Location = existingAccount.Location,
                    Email = existingAccount.Email,
                    Username = existingAccount.Username,
                    Occupation = existingAccount.Occupation,
                });
            }
            catch (DomainException ex)
            {
                _logger.LogError(ex.Message);
                return RedirectToAction("Login", "Accounts");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return RedirectToAction("ServerError");
            }
        }

        [HttpPost]
        public IActionResult Edit(EditProfileViewModel model)
        {
            try
            {
                var _currentUser = _context.Users.Find(userManager.GetUserId(User));
                _currentUser.Email = model.Email;
                _currentUser.UserName = model.Username;

                var existingAccount = _userStore.GetByIdentityUserId(_currentUser.Id);
                if (_currentUser is null)
                {
                    return RedirectToAction(StringValue.Login);
                }
                if (ModelState.IsValid)
                {


                    _userStore.Edit(existingAccount, model);

                    return RedirectToAction(StringValue.Profile);

                }
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return RedirectToAction(StringValue.ServerError);
            }



        }


        [HttpGet]
        public IActionResult Search()
        {
            try
            {
                var currentUser = _context.Users.Find(userManager.GetUserId(User));
                if (currentUser is null)
                    throw new DomainException(ErrorMessages.NotSignedIn);

                return View(new SearchUserDTO());
            }
            catch (DomainException ex)
            {
                _logger.LogError(ex.Message);
                return RedirectToAction(StringValue.Login);
            }



        }


        [HttpGet]
        public IActionResult NotFound()
        {
            return View();
        }



        [HttpPost]
        public IActionResult Search(SearchUserDTO searchUserDTO)
        {
            try
            {
                if (String.IsNullOrEmpty(searchUserDTO.Username))
                    throw new DomainException(ErrorMessages.RequiredValuesNotProvided);

                if (ModelState.IsValid)
                {
                    var ResultUser = _userStore.GetByUsername(searchUserDTO.Username);
                    var currentUser = _context.Users.Find(userManager.GetUserId(User));
                    if (ResultUser is null)
                        throw new DomainException(ErrorMessages.UserDoesNotExist);

                    else
                    {

                        var existingAccount = _userStore.GetByIdentityUserId(currentUser.Id);
                        var FriendList = _FriendListStore.GetFriendListOfUser(existingAccount.UserId);
                        var ResultFriendList = _FriendListStore.GetFriendListOfUser(ResultUser.UserId);
                        var areFriends = _FriendListStore.IsFriendsOfUser(FriendList.Id, ResultUser.UserId);
                        var ResultRequests = _RequestStore.PendingRequest(ResultFriendList.Id);
                        var CurrentUserRequests = _RequestStore.PendingRequest(FriendList.Id);
                        var SentByCurrent = ResultRequests.FirstOrDefault(x => x.Username == currentUser.UserName);
                        var SentByResult = CurrentUserRequests.FirstOrDefault(x => x.Username == ResultUser.Username);

                        var SearchResultDTO = new SearchUserResultDTO
                        {
                            Id = ResultUser.Id,
                            Firstname = ResultUser.FirstName,
                            Lastname = ResultUser.LastName,
                            Username = ResultUser.Username,
                            UserType = ResultUser.UserType,
                            UserId = ResultUser.UserId,
                            isFriendsOfCurrentUser = (areFriends is true) ? true : false,
                            ResultUserFriendListID = ResultFriendList.Id,
                            RequestIsPending = (areFriends is false && (SentByCurrent != null || SentByResult != null))
                        };
                        searchUserDTO.Users.Add(SearchResultDTO);

                    }
                }
                return View(searchUserDTO);
            }
            catch (DomainException ex)
            {
                _logger.LogError(ex.Message);
                ModelState.AddModelError("Error", ex.Message);
                return View(searchUserDTO);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return RedirectToAction(StringValue.ServerError);
            }

        }


        [HttpGet]
        public IActionResult UserProfile(Guid Id)
        {
            try
            {
                var existingAccount = _userStore.GetById(Id);
                if (existingAccount is null)
                    throw new DomainException(ErrorMessages.UserDoesNotExist);

                var FriendList = _FriendListStore.GetFriendListOfUser(existingAccount.UserId);
                var FriendUsers = _FriendListStore.GetFriendsOfUser(FriendList.Id);

                var ProfileViewModel = new ProfileViewModel
                {
                    FirstName = existingAccount.FirstName,
                    LastName = existingAccount.LastName,
                    Location = existingAccount.Location,
                    Email = existingAccount.Email,
                    Username = existingAccount.Username,
                    Occupation = existingAccount.Occupation,
                    UserType = existingAccount.UserType,
                    LastActive = existingAccount.LastActive,
                    JoinDate = existingAccount.DateJoined,
                    Groups = GroupStore.GetAllGroupsByUser(existingAccount.UserId)
                };
                foreach (var friend in FriendUsers)
                {
                    var account = _userStore.GetByUsername(friend.Username);
                    var FriendDTO = new FriendDTO()
                    {
                        Id = friend.Id,
                        UserId = friend.UserId,
                        Username = friend.Username,
                        AccountId = account.Id
                    };
                    ProfileViewModel.Friends.Add(FriendDTO);
                }
                return View(ProfileViewModel);
            }
            catch (DomainException ex)
            {
                _logger.LogError(ex.Message);
                return View(StringValue.NotFound);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return RedirectToAction(StringValue.ServerError);
            }


        }

        [HttpGet]
        public IActionResult Notifications()
        {
            var CurrentUser = _context.Users.Find(userManager.GetUserId(User));
            if (CurrentUser is null)
                return RedirectToAction(StringValue.Login);
            var NotificationsViewDTO = new NotificationsViewDTO() { };
            var Notifications = _notificationBox.GetUserNotifications(CurrentUser.UserName);
            foreach (var notification in Notifications)
            {
                NotificationsViewDTO.AllNotifications.Add(new NotificationDTO()
                {
                    Id = notification.Id,
                    Message = notification.Message,
                    NotificationBoxId = notification.NotificationBoxId,
                    RecieverUsername = notification.RecieverUsername
                });
            }

            return View(NotificationsViewDTO);
        }

        public IActionResult DeleteNotification(Guid Id)
        {
            var existingNotification = _notificationBox.GetNotificationById(Id);
            try
            {
                if (existingNotification is null)
                    throw new DomainException();


                _notificationBox.DeleteNotifcation(existingNotification);
                return RedirectToAction("Notifications");
            }
            catch (DomainException ex)
            {
                return RedirectToAction("NotFound", "Accounts");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return RedirectToAction("ServerError");
            }


        }



        [HttpGet]
        public IActionResult GrantAdminAccess(Guid Id)
        {
            var existingAccount = _userStore.GetById(Id);
            var AccountDTO = new AccountDTO();
            AccountDTO.UserId = existingAccount.UserId;
            return View(AccountDTO);
        }

        [HttpPost]
        public IActionResult GrantAdminAccess(AccountDTO accountDTO)
        {
            var existingAccount = _userStore.GetById(accountDTO.Id);
            _userStore.GrantAdmin(existingAccount);
            var Notification = new Notification()
            {
                Message = "You have been granted Admin access ",
                NotificationBoxId = _notificationBox.GetNotificationBoxByUserId(existingAccount.UserId).Id,
                RecieverUsername = _userStore.GetByIdentityUserId(existingAccount.UserId).Username
            };


            _notificationBox.SendNotifcation(Notification);
            _context.SaveChanges();
            return RedirectToAction("AdminPortal");
        }

        [HttpGet]
        public IActionResult RevokeAdminAccess(Guid Id)
        {
            var existingAccount = _userStore.GetById(Id);
            var AccountDTO = new AccountDTO();
            AccountDTO.UserId = existingAccount.UserId;
            return View(AccountDTO);
        }

        [HttpPost]
        public IActionResult RevokeAdminAccess(AccountDTO accountDTO)
        {
            var existingAccount = _userStore.GetById(accountDTO.Id);
            existingAccount.UserType = UserType.User;
            var Notification = new Notification()
            {
                Message = "Your Admin permissions have been Revoked ",
                NotificationBoxId = _notificationBox.GetNotificationBoxByUserId(existingAccount.UserId).Id,
                RecieverUsername = _userStore.GetByIdentityUserId(existingAccount.UserId).Username
            };


            _notificationBox.SendNotifcation(Notification);
            _context.SaveChanges();
            return RedirectToAction("AdminPortal");
        }

        [HttpGet]
        public IActionResult ServerError()
        {

            return View();
        }
    }

}
