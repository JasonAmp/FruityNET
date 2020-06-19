using System;
using FruityNET.Data;
using FruityNET.DTOs;
using FruityNET.Entities;
using FruityNET.Enums;
using FruityNET.IEntityStore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using FruityNET.Models;
using Microsoft.Extensions.Logging;
using FruityNET.ParameterStrings;
using FruityNET.Exceptions;

namespace FruityNET.Controllers
{
    public class GroupsController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        private readonly ApplicationDbContext _context;
        private readonly IUserStore _userStore;
        private readonly IGroupStore _GroupStore;
        private readonly ILogger<GroupsController> _logger;


        public GroupsController(UserManager<User> userManager, SignInManager<User> signInManager, ApplicationDbContext _context,
        IUserStore _userStore, IGroupStore _GroupStore, ILogger<GroupsController> _logger)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this._context = _context;
            this._userStore = _userStore;
            this._GroupStore = _GroupStore;
            this._logger = _logger;
        }

        [HttpGet]
        public IActionResult AddGroup()
        {
            var CurrentUser = _context.Users.Find(userManager.GetUserId(User));
            var existingAccount = _userStore.GetByIdentityUserId(CurrentUser.Id);
            if (existingAccount.AccountStatus.Equals(Status.Suspended))
                signInManager.SignOutAsync();

            return View();
        }

        [HttpPost]
        public IActionResult AddGroup(AddGroupDTO AddGroupDTO)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var CurrentUser = _context.Users.Find(userManager.GetUserId(User));
                    var existingAccount = _userStore.GetByIdentityUserId(CurrentUser.Id);
                    if (existingAccount.AccountStatus.Equals(Status.Suspended))
                        signInManager.SignOutAsync();
                    var Group = new Group()
                    {
                        UserId = CurrentUser.Id,
                        Name = AddGroupDTO.Name,
                        Description = AddGroupDTO.Description,
                        CreationDate = DateTime.Now
                    };
                    _GroupStore.CreateGroup(Group);
                    var GroupOwner = new GroupOwner()
                    {
                        UserId = CurrentUser.Id,
                        Username = CurrentUser.UserName,
                        GroupId = Group.Id
                    };
                    var GroupUser = new GroupUser()
                    {
                        UserId = CurrentUser.Id,
                        Username = CurrentUser.UserName,
                        GroupId = Group.Id
                    };
                    _GroupStore.CreateGroupOwner(GroupOwner);
                    _GroupStore.CreateGroupUser(GroupUser);
                    return RedirectToAction("Profile", "Accounts");
                }
                return View(AddGroupDTO);
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
        public IActionResult GroupDetails(Guid Id)
        {
            try
            {
                var CurrentUser = _context.Users.Find(userManager.GetUserId(User));
                if (CurrentUser is null)
                    throw new DomainException(ErrorMessages.NotSignedIn);
                var existingAccount = _userStore.GetByIdentityUserId(CurrentUser.Id);
                if (existingAccount.AccountStatus.Equals(Status.Suspended))
                    signInManager.SignOutAsync();

                var existingGroup = _GroupStore.GetGroupById(Id);
                if (existingGroup is null)
                    throw new DomainException(ErrorMessages.GroupDoesNotExist);

                var groupMembers = _GroupStore.GetGroupMembers(existingGroup.Id);
                var Owner = _GroupStore.GetGroupOwner(existingGroup.Id);

                var GroupDetailsDTO = new GroupDetailsDTO()
                {
                    Id = existingGroup.Id,
                    Name = existingGroup.Name,
                    Description = existingGroup.Description,
                    CreationDate = existingGroup.CreationDate,
                    GroupOwner = Owner.Username
                };
                foreach (var member in groupMembers)
                {
                    var GroupMemberDTO = new GroupMemberDTO()
                    {
                        Id = member.Id,
                        Username = member.Username,
                        UserId = member.UserId,
                        Type = member.Type
                    };
                    GroupDetailsDTO.GroupMembers.Add(GroupMemberDTO);
                }
                return View(GroupDetailsDTO);
            }
            catch (DomainException ex)
            {
                _logger.LogError(ex.Message);
                if (ex.Message.Equals(ErrorMessages.NotSignedIn))
                    return RedirectToAction(ActionName.Login, ControllerName.Accounts);

                return RedirectToAction(ActionName.NotFound, ControllerName.Accounts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return RedirectToAction(ActionName.ServerError, ControllerName.Accounts);
            }


        }


        public IActionResult AddGroupMember(Guid Id)
        {
            try
            {
                var CurrentUser = _context.Users.Find(userManager.GetUserId(User));
                if (CurrentUser is null)
                    throw new DomainException(ErrorMessages.NotSignedIn);
                var existingAccount = _userStore.GetByIdentityUserId(CurrentUser.Id);
                if (existingAccount.AccountStatus.Equals(Status.Suspended))
                    signInManager.SignOutAsync();
                return View(new AddGroupUserDTO { Id = Id });

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


        [HttpPost]
        public IActionResult AddGroupMember(AddGroupUserDTO addGroupUserDTO)
        {
            try
            {
                if (String.IsNullOrEmpty(addGroupUserDTO.Username))
                    throw new DomainException(ErrorMessages.UserNotProvided);

                if (ModelState.IsValid)
                {
                    var existingUser = _userStore.GetByUsername(addGroupUserDTO.Username);
                    if (existingUser is null)
                        throw new DomainException(ErrorMessages.UserDoesNotExist);

                    var existingGroupMember = (existingUser != null) ? _GroupStore.GetGroupMembers(addGroupUserDTO.Id)
                    .FirstOrDefault(x => x.Username == addGroupUserDTO.Username) : null;

                    if (existingGroupMember != null)
                        throw new DomainException(ErrorMessages.GroupUserExists);


                    else if (existingUser != null && existingGroupMember is null)
                    {
                        var GroupUser = new GroupUser()
                        {
                            UserId = existingUser.UserId,
                            GroupId = addGroupUserDTO.Id,
                            Username = addGroupUserDTO.Username,
                            Type = GroupUserType.Member
                        };
                        _GroupStore.CreateGroupUser(GroupUser);
                        ViewBag.Message = "Success";
                        return View(addGroupUserDTO);
                    }
                }
                return View(addGroupUserDTO);
            }
            catch (DomainException ex)
            {
                _logger.LogError(ex.Message);
                ModelState.AddModelError("Error", ex.Message);
                return View(addGroupUserDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return RedirectToAction(ActionName.ServerError, ControllerName.Accounts);
            }
        }
        [HttpGet]
        public IActionResult SearchGroup()
        {
            try
            {
                var CurrentUser = _context.Users.Find(userManager.GetUserId(User));
                if (CurrentUser is null)
                    throw new DomainException(ErrorMessages.NotSignedIn);

                var existingAccount = _userStore.GetByIdentityUserId(CurrentUser.Id);
                if (existingAccount.AccountStatus.Equals(Status.Suspended))
                    signInManager.SignOutAsync();

                return View(new SearchGroupViewModel());
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
        [HttpPost]
        public IActionResult SearchGroup(SearchGroupViewModel searchGroupViewModel)
        {
            if (ModelState.IsValid)
            {
                var existingGroup = _GroupStore.GetGroupByName(searchGroupViewModel.Groupname);
                if (existingGroup is null)
                    ViewBag.Message = "Group Does not Exist";
                else
                {
                    var GroupResult = new GroupResultViewModel()
                    {
                        Groupname = existingGroup.Name,
                        GroupId = existingGroup.Id,
                    };
                    searchGroupViewModel.Groups.Add(GroupResult);
                }
            }
            return View(searchGroupViewModel);
        }



        [HttpGet]
        public IActionResult Delete(Guid Id)
        {
            try
            {
                var CurrentUser = _context.Users.Find(userManager.GetUserId(User));
                var existingAccount = _userStore.GetByIdentityUserId(CurrentUser.Id);
                if (existingAccount.AccountStatus.Equals(Status.Suspended))
                    signInManager.SignOutAsync();

                var existingGroup = _GroupStore.GetGroupById(Id);
                if (existingGroup is null)
                    throw new DomainException(ErrorMessages.GroupDoesNotExist);
                return View(existingGroup);


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
        [HttpPost]
        public IActionResult Delete(Group group)
        {
            if (ModelState.IsValid)
            {
                var GroupUsers = _GroupStore.GetGroupMembers(group.Id);
                foreach (var user in GroupUsers)
                {
                    _GroupStore.DeleteGroupUser(user);
                }
                _GroupStore.DeleteGroup(group);

            }
            return RedirectToAction(ActionName.Profile, ControllerName.Accounts);

        }


        [HttpGet]
        public IActionResult RemoveUser(Guid Id)
        {
            try
            {
                var CurrentUser = _context.Users.Find(userManager.GetUserId(User));
                var existingAccount = _userStore.GetByIdentityUserId(CurrentUser.Id);
                if (existingAccount.AccountStatus.Equals(Status.Suspended))
                    signInManager.SignOutAsync();

                var existingGroupMember = _GroupStore.GetGroupMemberById(Id);
                if (existingGroupMember is null)
                    throw new DomainException(ErrorMessages.UserDoesNotExist);

                var GroupID = existingGroupMember.GroupId.ToString().Clone();
                var GroupMemberDTO = new GroupMemberDTO()
                {
                    GroupId = new Guid(GroupID.ToString()),
                    Id = existingGroupMember.Id,
                    Username = existingGroupMember.Username
                };
                return View(GroupMemberDTO);
            }
            catch (DomainException ex)
            {
                _logger.LogError(ex.Message);
                return RedirectToAction(ActionName.NotFound, ControllerName.Accounts);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return RedirectToAction(ActionName.ServerError, ControllerName.Accounts);
            }
        }
        [HttpPost]
        public IActionResult RemoveUser(GroupMemberDTO GroupMemberDTO)
        {
            try
            {
                var existingGroupMember = _GroupStore.GetGroupMemberById(GroupMemberDTO.Id);
                var GroupID = new Guid(existingGroupMember.GroupId.ToString());


                _GroupStore.DeleteGroupUser(existingGroupMember);
                return RedirectToAction("GroupDetails", "Groups", new { id = GroupID });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return RedirectToAction(ActionName.ServerError, ControllerName.Accounts);
            }
        }
    }
}