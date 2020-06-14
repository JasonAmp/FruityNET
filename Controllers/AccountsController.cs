using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FruityNET.Data;
using FruityNET.DTOs;
using FruityNET.Entities;
using FruityNET.Enums;
using FruityNET.IEntityStore;
using FruityNET.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

using Microsoft.AspNetCore.Mvc;

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






        public AccountsController(UserManager<User> userManager, SignInManager<User> signInManager, ApplicationDbContext _context,
        IUserStore _userStore, IFriendsListStore _FriendListStore, IRequestStore _RequestStore, INotificationBox _notificationBox,
        IGroupStore GroupStore)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this._context = _context;
            this._userStore = _userStore;
            this._FriendListStore = _FriendListStore;
            this._RequestStore = _RequestStore;
            this._notificationBox = _notificationBox;
            this.GroupStore = GroupStore;
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

                var result = await signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, false);

                if (result.Succeeded)
                    return RedirectToAction("Index", "Home");

            }
            ModelState.AddModelError("Error", "Invalid Login Attempt");
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
            return View(model);

        }

        public UserAccount CreateAccount(UserAccount userAccount)
        {
            _userStore.CreateAccount(userAccount);
            return userAccount;

        }


        public IActionResult SignOut()
        {
            signInManager.SignOutAsync();
            return RedirectToAction("Login", "Accounts");
        }



        [HttpGet]
        public IActionResult Profile()
        {
            var _currentUser = _context.Users.Find(userManager.GetUserId(User));
            if (_currentUser is null)
            {
                return RedirectToAction("Login", "Accounts");
            }
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
                LastActive = DateTime.Now,
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

        public IActionResult Edit()
        {
            var _currentUser = _context.Users.Find(userManager.GetUserId(User));
            if (_currentUser is null)
                return RedirectToAction("Login", "Accounts");


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

        [HttpPost]
        public IActionResult Edit(EditProfileViewModel model)
        {
            var _currentUser = _context.Users.Find(userManager.GetUserId(User));
            _currentUser.Email = model.Email;
            _currentUser.UserName = model.Username;

            var existingAccount = _userStore.GetByIdentityUserId(_currentUser.Id);
            if (_currentUser is null)
            {
                return RedirectToAction("Login", "Accounts");
            }
            if (ModelState.IsValid)
            {


                _userStore.Edit(existingAccount, model);

                return RedirectToAction("Profile", "Accounts");

            }
            return View(model);

        }


        [HttpGet]
        public IActionResult Search()
        {

            var currentUser = _context.Users.Find(userManager.GetUserId(User));
            if (currentUser is null)
                return RedirectToAction("Login");

            return View(new SearchUserDTO());
        }


        [HttpGet]
        public IActionResult NotFound()
        {
            return View();
        }



        [HttpPost]
        public IActionResult Search(SearchUserDTO searchUserDTO)
        {
            if (String.IsNullOrEmpty(searchUserDTO.Username))
                ModelState.AddModelError("Error", "Please provide a Username.");

            if (ModelState.IsValid)
            {
                var ResultUser = _userStore.GetByUsername(searchUserDTO.Username);
                var currentUser = _context.Users.Find(userManager.GetUserId(User));
                if (ResultUser is null)
                    ModelState.AddModelError("Error", "User Does Not Exist.");

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


        [HttpGet]
        public IActionResult ViewProfileOfUser(Guid Id)
        {
            var existingAccount = _userStore.GetById(Id);
            if (existingAccount is null)
                return View("NotFound");

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

        [HttpGet]
        public IActionResult Notifications()
        {
            var _currentUser = _context.Users.Find(userManager.GetUserId(User));
            var NotificationsViewDTO = new NotificationsViewDTO() { };
            var Notifications = _notificationBox.GetUserNotifications(_currentUser.UserName);
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
            if (existingNotification is null)
                return RedirectToAction("NotFound", "Accounts");
            _notificationBox.DeleteNotifcation(existingNotification);
            return RedirectToAction("Notifications");
        }
    }

}
