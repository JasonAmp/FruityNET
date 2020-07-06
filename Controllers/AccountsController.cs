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
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using FruityNET.Queries;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

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
        private readonly IAdminRequestStore _AdminRequestStore;


        public AccountsController(UserManager<User> userManager, SignInManager<User> signInManager, ApplicationDbContext _context,
        IUserStore _userStore, IFriendsListStore _FriendListStore, IRequestStore _RequestStore, INotificationBox _notificationBox,
        IGroupStore GroupStore, ILogger<AccountsController> _logger, IAdminRequestStore _AdminRequestStore)
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
            this._AdminRequestStore = _AdminRequestStore;
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
                if (existingAccount.AccountStatus.Equals(Status.Suspended))
                    signInManager.SignOutAsync();


                if (existingAccount.UserType != UserType.Admin && existingAccount.UserType != UserType.SiteOwner)
                    throw new ForbiddenException(ErrorMessages.ForbiddenAccess);

                var AdminRequests = _AdminRequestStore.GetAll();
                var AdminPortalDTO = new AdminPortalViewDTO()
                {
                    RequestCount = AdminRequests.Count,
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
                        Email = user.Email,
                        AccountStatus = user.AccountStatus
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
                    var existingAccount = _userStore.GetByUsername(model.UserName);
                    if (existingAccount is null)
                        throw new DomainException(ErrorMessages.InvalidLogin);

                    if (existingAccount.AccountStatus != Status.Active)
                    {
                        if (existingAccount.AccountStatus.Equals(Status.Suspended))
                            throw new DomainException(ErrorMessages.AccountSuspended);
                    }

                    var user = await userManager.FindByIdAsync(existingAccount.UserId);
                    var result = await userManager.CheckPasswordAsync(user, model.Password);

                    if (result is false)
                        throw new DomainException(ErrorMessages.InvalidLogin);

                    SignUserIn(user, existingAccount);
                    return RedirectToAction("Index", "Home");
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
        protected async void SignUserIn(User user, UserAccount existingAccount)
        {
            var identity = new ClaimsIdentity(IdentityConstants.ApplicationScheme);
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id));
            identity.AddClaim(new Claim(ClaimTypes.Name, user.UserName));
            await HttpContext.SignInAsync(IdentityConstants.ApplicationScheme, new ClaimsPrincipal(identity));
            existingAccount.LastActive = DateTime.Now;
            _context.SaveChanges();
        }
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var UserWithEmail = _userStore.GetAccounts().First(x => x.Email.Equals(model.Email));
                    if (UserWithEmail != null)
                    {
                        _logger.LogError($"Email '{model.Email}' is taken.");
                        ModelState.AddModelError("Error", $"Email '{model.Email}' is taken.");
                    }

                    var user = new User
                    {
                        UserName = model.UserName,
                        Email = model.Email,
                        UserType = UserType.User
                    };

                    var result = await userManager.CreateAsync(user, model.Password);
                    if (result.Succeeded)
                    {
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
                            AccountStatus = Status.Inactive
                        };

                        var UserFriendList = _FriendListStore.CreateFriendList(new FriendList() { UserId = user.Id });
                        var NotificationBox = _notificationBox.CreateNotificationBox(new NotificationBox() { UserId = user.Id });
                        userAccount.FriendList = UserFriendList.Users;
                        CreateAccount(userAccount);
                        ViewBag.Message = "Success";
                        return View(model);

                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("Error", error.Description);
                        _logger.LogError(error.Description);
                    }
                }
                if (model.Password != model.ConfirmPassword)
                {
                    _logger.LogError("Password confirmation failed");
                    ModelState.AddModelError("Error", "Password confirmation failed");
                }
                else if (String.IsNullOrEmpty(model.UserName) || String.IsNullOrEmpty(model.FirstName)
                || String.IsNullOrEmpty(model.LastName) || String.IsNullOrEmpty(model.Email)
                || String.IsNullOrEmpty(model.Password) || String.IsNullOrEmpty(model.ConfirmPassword)
                || String.IsNullOrEmpty(model.UserName))
                {
                    ModelState.AddModelError("Error", ErrorMessages.RequiredValuesNotProvided);
                }
                return View(model);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return RedirectToAction("ServerError");
            }

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
                if (existingAccount.AccountStatus.Equals(Status.Suspended))
                    signInManager.SignOutAsync();

                var FriendList = _FriendListStore.GetFriendListOfUser(_currentUser.Id);
                var FriendUsers = _FriendListStore.GetFriendsOfUser(FriendList.Id);
                var GroupsWithUser = GroupStore.GetGroupsWithUser(existingAccount.UserId);
                var NotificationCount = _notificationBox.GetUserNotifications(existingAccount.Username).Count
                + _FriendListStore.GetIncomingFriendRequests(FriendList.Id).Count;

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
                    Groups = GroupsWithUser,
                    UserId = _currentUser.Id,
                    NotificationCount = NotificationCount
                };
                foreach (var friend in FriendUsers)
                {
                    var account = _userStore.GetByUsername(friend.Username);
                    if (account.AccountStatus.Equals(Status.Active))
                    {
                        var FriendDTO = new FriendDTO()
                        {
                            Id = friend.Id,
                            UserId = friend.UserId,
                            Username = friend.Username,
                            AccountId = account.Id
                        };
                        ProfileViewModel.Friends.Add(FriendDTO);
                    }

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
                var CurrentUser = _context.Users.Find(userManager.GetUserId(User));
                if (CurrentUser is null)
                    throw new DomainException(ErrorMessages.NotSignedIn);

                var existingAccount = _userStore.GetByIdentityUserId(CurrentUser.Id);
                if (existingAccount.AccountStatus.Equals(Status.Suspended))
                    signInManager.SignOutAsync();

                return View(new EditProfileViewModel
                {
                    UserId = CurrentUser.Id,
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
                if (_currentUser is null)
                    throw new DomainException(ErrorMessages.NotSignedIn);


                var existingUser = _userStore.GetAccounts().FirstOrDefault(x => x.Username.Equals(model.Username));
                if (existingUser != null && !existingUser.UserId.Equals(_currentUser.Id))
                    throw new DomainException(ErrorMessages.UserAlreadyExists);


                _currentUser.Email = model.Email;
                _currentUser.UserName = model.Username;

                var existingAccount = _userStore.GetByIdentityUserId(_currentUser.Id);
                if (_currentUser is null)
                {
                    return RedirectToAction(ActionName.Login);
                }
                if (ModelState.IsValid)
                {
                    _userStore.Edit(existingAccount, model);
                    return RedirectToAction(ActionName.Profile);
                }
                return View(model);
            }
            catch (DomainException ex)
            {
                _logger.LogError(ex.Message);
                if (ex.Message.Equals(ErrorMessages.NotSignedIn))
                    return RedirectToAction(ActionName.Login, ControllerName.Accounts);
                else
                {
                    ViewBag.Message = ex.Message;
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return RedirectToAction(ActionName.ServerError);
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

                var existingAccount = _userStore.GetByIdentityUserId(currentUser.Id);

                var GetAllUsers = new GetAllUsersQuery(_userStore, _FriendListStore,
                _RequestStore, currentUser, existingAccount, signInManager);

                return View(GetAllUsers.Handle());
            }
            catch (DomainException ex)
            {
                _logger.LogError(ex.Message);
                return RedirectToAction(ActionName.Login);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return RedirectToAction(ActionName.ServerError);
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

                    if (ResultUser is null || ResultUser.AccountStatus.Equals(Status.Suspended)
                    || ResultUser.AccountStatus.Equals(Status.Inactive))
                        throw new DomainException(ErrorMessages.UserDoesNotExist);

                    var GetSearchResultQuery = new GetSearchResultQuery(_userStore, _FriendListStore,
                    _RequestStore, currentUser, ResultUser, searchUserDTO);
                    return View(GetSearchResultQuery.Handle());
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
                return RedirectToAction(ActionName.ServerError);
            }
        }




        [HttpGet]
        public IActionResult UserProfile(Guid Id)
        {
            try
            {
                var existingAccount = _userStore.GetById(Id);
                var currentUser = _context.Users.Find(userManager.GetUserId(User));

                if (existingAccount.UserId.Equals(currentUser.Id))
                    return RedirectToAction(ActionName.Profile);

                var GetUserProfileQuery = new GetUserProfileQuery(Id, userManager, _userStore, _FriendListStore,
                _RequestStore, GroupStore, _logger, existingAccount, currentUser);

                return View(GetUserProfileQuery.Handle());
            }
            catch (DomainException ex)
            {
                _logger.LogError(ex.Message);
                if (ex.Message.Equals(ErrorMessages.NotSignedIn))
                    return RedirectToAction(ActionName.Login);
                return View(ActionName.NotFound);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return RedirectToAction(ActionName.ServerError);
            }
        }

        [HttpGet]
        public IActionResult Notifications()
        {
            try
            {
                var CurrentUser = _context.Users.Find(userManager.GetUserId(User));
                if (CurrentUser is null)
                    throw new DomainException(ErrorMessages.NotSignedIn);

                var existingAccount = _userStore.GetByIdentityUserId(CurrentUser.Id);
                if (existingAccount.AccountStatus.Equals(Status.Suspended))
                    signInManager.SignOutAsync();

                var GetAllNotifications = new GetAllNotificationsQuery(_userStore, _notificationBox, existingAccount, CurrentUser);
                return View(GetAllNotifications.Handle());
            }
            catch (DomainException ex)
            {
                _logger.LogError(ex.Message);
                return RedirectToAction(ActionName.Login);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return RedirectToAction("ServerError");
            }
        }

        public IActionResult DeleteNotification(Guid Id)
        {
            try
            {
                var CurrentUser = _context.Users.Find(userManager.GetUserId(User));
                if (CurrentUser is null)
                    throw new DomainException(ErrorMessages.NotSignedIn);

                var existingNotification = _notificationBox.GetNotificationById(Id);
                if (existingNotification is null)
                    throw new DomainException();


                _notificationBox.DeleteNotifcation(existingNotification);
                return RedirectToAction("Notifications");
            }
            catch (DomainException ex)
            {
                _logger.LogError(ex.Message);
                if (ex.Message.Equals(ErrorMessages.NotSignedIn))
                    return RedirectToAction(ActionName.Login);
                return RedirectToAction("NotFound", "Accounts");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return RedirectToAction("ServerError");
            }


        }

        [HttpGet]
        public IActionResult PermissionExists()
        {

            return View();
        }



        [HttpGet]
        public IActionResult GrantAdminAccess(Guid Id)
        {
            try
            {
                var CurrentUser = _context.Users.Find(userManager.GetUserId(User));
                if (CurrentUser is null)
                    throw new DomainException(ErrorMessages.NotSignedIn);



                var CurrentUserAccount = _userStore.GetByIdentityUserId(CurrentUser.Id);
                if (CurrentUserAccount.UserType.Equals(UserType.User))
                    throw new ForbiddenException(ErrorMessages.ForbiddenAccess);

                var existingAccount = _userStore.GetById(Id);
                if (existingAccount is null || existingAccount.AccountStatus.Equals(Status.Suspended)
                || existingAccount.AccountStatus.Equals(Status.Inactive))
                    throw new DomainException(ErrorMessages.UserDoesNotExist);

                if (CurrentUserAccount.AccountStatus.Equals(Status.Suspended))
                    signInManager.SignOutAsync();

                if (existingAccount.UserType != UserType.Admin && existingAccount.UserType != UserType.SiteOwner)
                {
                    var AccountDTO = new AccountDTO();
                    AccountDTO.UserId = existingAccount.UserId;
                    return View(AccountDTO);
                }
                else
                {
                    throw new DomainException(ErrorMessages.AdminExists);
                }

            }
            catch (DomainException ex)
            {
                _logger.LogError(ex.Message);
                if (ex.Message.Equals(ErrorMessages.AdminExists))
                    return RedirectToAction(ActionName.PermissionExists);

                else if (ex.Message.Equals(ErrorMessages.NotSignedIn))
                    return RedirectToAction(ActionName.Login);

                return RedirectToAction(ActionName.NotFound);
            }
            catch (ForbiddenException ex)
            {
                _logger.LogError(ex.Message);
                return RedirectToAction(ActionName.NotAuthorized);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return RedirectToAction(ActionName.ServerError);
            }


        }

        [HttpPost]
        public IActionResult GrantAdminAccess(AccountDTO accountDTO)
        {
            try
            {
                var existingAccount = _userStore.GetById(accountDTO.Id);
                _userStore.GrantAdmin(existingAccount);
                var Notification = new Notification()
                {
                    Message = "You have been granted Admin access ",
                    NotificationBoxId = _notificationBox.GetNotificationBoxByUserId(existingAccount.UserId).Id,
                    RecieverUsername = _userStore.GetByIdentityUserId(existingAccount.UserId).Username,
                    NotificationDate = DateTime.Now

                };
                _notificationBox.SendNotifcation(Notification);
                _context.SaveChanges();
                return RedirectToAction("AdminPortal");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return RedirectToAction(ActionName.ServerError);
            }


        }

        [HttpGet]
        public IActionResult RevokeAdminAccess(Guid Id)
        {
            try
            {
                var CurrentUser = _context.Users.Find(userManager.GetUserId(User));
                if (CurrentUser is null)
                    throw new DomainException(ErrorMessages.NotSignedIn);

                var CurrentUserAccount = _userStore.GetByIdentityUserId(CurrentUser.Id);
                if (CurrentUserAccount.UserType.Equals(UserType.User))
                    throw new ForbiddenException(ErrorMessages.ForbiddenAccess);

                if (CurrentUserAccount.AccountStatus.Equals(Status.Suspended))
                    signInManager.SignOutAsync();

                var existingAccount = _userStore.GetById(Id);


                if (existingAccount is null || existingAccount.AccountStatus.Equals(Status.Suspended)
                || existingAccount.AccountStatus.Equals(Status.Inactive))
                    throw new DomainException(ErrorMessages.UserDoesNotExist);


                var AccountDTO = new AccountDTO();

                AccountDTO.UserId = existingAccount.UserId;
                return View(AccountDTO);
            }
            catch (DomainException ex)
            {
                _logger.LogError(ex.Message);
                if (ex.Message.Equals(ErrorMessages.NotSignedIn))
                    return RedirectToAction(ActionName.Login);

                if (ex.Message.Equals(ErrorMessages.ForbiddenAccess))
                    return RedirectToAction(ActionName.NotAuthorized);

                return RedirectToAction(ActionName.NotFound);
            }
            catch (ForbiddenException ex)
            {
                _logger.LogError(ex.Message);
                return RedirectToAction(ActionName.NotAuthorized);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return RedirectToAction(ActionName.ServerError);
            }


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
                RecieverUsername = _userStore.GetByIdentityUserId(existingAccount.UserId).Username,
                NotificationDate = DateTime.Now
            };


            _notificationBox.SendNotifcation(Notification);
            _context.SaveChanges();
            return RedirectToAction("AdminPortal");
        }

        [HttpGet]
        public IActionResult ServerError()
        {
            ViewBag.Message = StatusCodes.Status500InternalServerError.ToString();
            return View();
        }


        [HttpGet]
        public IActionResult Activate(Guid Id)
        {
            try
            {
                var CurrentUser = _context.Users.Find(userManager.GetUserId(User));
                if (CurrentUser is null)
                    throw new DomainException(ErrorMessages.NotSignedIn);

                var CurrentUserAccount = _userStore.GetByIdentityUserId(CurrentUser.Id);
                if (CurrentUserAccount.UserType.Equals(UserType.User))
                    throw new ForbiddenException(ErrorMessages.ForbiddenAccess);

                var existingAccount = _userStore.GetById(Id);
                if (existingAccount is null)
                    throw new DomainException(ErrorMessages.UserDoesNotExist);

                return View(new ActivatetDTO() { AccountID = Id });
            }
            catch (DomainException ex)
            {
                _logger.LogError(ex.Message);
                if (ex.Message.Equals(ErrorMessages.UserDoesNotExist))
                    return RedirectToAction(ActionName.NotFound);

                return RedirectToAction(ActionName.Login);
            }
            catch (ForbiddenException ex)
            {
                _logger.LogError(ex.Message);
                return RedirectToAction(ActionName.NotAuthorized);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return RedirectToAction(ActionName.ServerError);
            }

        }

        [HttpPost]
        public IActionResult ConfirmActivate(Guid Id)
        {
            try
            {
                var existingAccount = _userStore.GetById(Id);
                existingAccount.AccountStatus = Status.Active;
                _context.SaveChanges();
                return RedirectToAction("AdminPortal");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return RedirectToAction(ActionName.ServerError);
            }


        }


        [HttpGet]
        public IActionResult Suspend(Guid Id)
        {
            try
            {
                var CurrentUser = _context.Users.Find(userManager.GetUserId(User));
                if (CurrentUser is null)
                    throw new DomainException(ErrorMessages.NotSignedIn);

                var CurrentUserAccount = _userStore.GetByIdentityUserId(CurrentUser.Id);
                if (CurrentUserAccount.UserType.Equals(UserType.User))
                    throw new ForbiddenException(ErrorMessages.ForbiddenAccess);

                var existingAccount = _userStore.GetById(Id);
                if (existingAccount is null || existingAccount.AccountStatus.Equals(Status.Suspended)
                || existingAccount.AccountStatus.Equals(Status.Inactive))
                    throw new DomainException(ErrorMessages.UserDoesNotExist);

                return View(new ActivatetDTO() { AccountID = Id });
            }
            catch (DomainException ex)
            {
                _logger.LogError(ex.Message);
                if (ex.Message.Equals(ErrorMessages.UserDoesNotExist))
                    return RedirectToAction(ActionName.NotFound);

                return RedirectToAction(ActionName.Login);
            }
            catch (ForbiddenException ex)
            {
                _logger.LogError(ex.Message);
                return RedirectToAction(ActionName.NotAuthorized);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return RedirectToAction(ActionName.ServerError);
            }


        }

        [HttpPost]
        public IActionResult ConfirmSuspend(Guid Id)
        {
            try
            {
                var existingAccount = _userStore.GetById(Id);
                existingAccount.AccountStatus = Status.Suspended;
                _context.SaveChanges();
                return RedirectToAction(ActionName.AdminPortal);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return RedirectToAction(ActionName.ServerError);
            }


        }

        [HttpGet]
        public IActionResult ResetPassword()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ResetPassword(ResetPasswordModel model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.UserName) || string.IsNullOrEmpty(model.Email)
                || string.IsNullOrWhiteSpace(model.Password) || string.IsNullOrWhiteSpace(model.ConfirmPassword))
                    throw new DomainException(ErrorMessages.RequiredValuesNotProvided);

                if (model.Password != model.ConfirmPassword)
                    throw new DomainException(ErrorMessages.passwordConfirmationFail);


                if (ModelState.IsValid)
                {
                    var existingUser = userManager.Users.FirstOrDefault(x => x.UserName.Equals(model.UserName));
                    if (existingUser is null)
                        throw new DomainException(ErrorMessages.UserDoesNotExist);
                    var existingAccount = _userStore.GetByIdentityUserId(existingUser.Id);
                    if (!model.Email.Equals(existingAccount.Email))
                        throw new DomainException(ErrorMessages.InvalidResetCredentials);

                    if (existingAccount.AccountStatus.Equals(Status.Suspended))
                        throw new DomainException(ErrorMessages.AccountSuspended);

                    var passwordHasher = new PasswordHasher<User>();
                    var NewPasswordHash = passwordHasher.HashPassword(existingUser, model.Password);
                    existingUser.PasswordHash = NewPasswordHash;
                    _context.SaveChanges();
                    ViewBag.Message = "Success";

                }

                return View(model);
            }
            catch (DomainException ex)
            {
                _logger.LogError(ex.Message);
                ModelState.AddModelError("Error", ex.Message);
                return View(model);
            }

            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return RedirectToAction(ActionName.ServerError);
            }


        }

        [HttpGet]
        public IActionResult DeleteAccount(string Id)
        {
            return View();
        }
    }

}
