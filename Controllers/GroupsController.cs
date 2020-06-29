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
using System.Collections.Generic;
using static System.Net.WebRequestMethods;
using System.Net;

namespace FruityNET.Controllers
{
    public class GroupsController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        private readonly ApplicationDbContext _context;
        private readonly IUserStore _userStore;
        private readonly IGroupStore _GroupStore;
        private readonly IGroupRequestStore _GroupRequestStore;
        private readonly ILogger<GroupsController> _logger;


        public GroupsController(UserManager<User> userManager, SignInManager<User> signInManager, ApplicationDbContext _context,
        IUserStore _userStore, IGroupStore _GroupStore, ILogger<GroupsController> _logger, IGroupRequestStore _GroupRequestStore)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this._context = _context;
            this._userStore = _userStore;
            this._GroupStore = _GroupStore;
            this._logger = _logger;
            this._GroupRequestStore = _GroupRequestStore;
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
                    _logger.LogInformation("Success: " + Group.Id.ToString() + " " + "Group Name: " + Group.Name);

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
                    return RedirectToAction("Profile", "Accounts", HttpStatusCode.OK);
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
                var Requests = _GroupRequestStore.GetGroupRequestsOfGroup(existingGroup.Id);
                var RequestDTOs = new List<GroupRequestDTO>();

                foreach (var request in Requests)
                {
                    var requestUser = _GroupRequestStore.GetGroupRequestUser(request.RequestUserId);
                    var GroupRequestDTO = new GroupRequestDTO()
                    {
                        Username = requestUser.Username,
                        Id = request.Id,
                        GroupID = request.GroupId,
                        RequestDate = request.RequestDate,
                        RequestUserId = request.RequestUserId
                    };
                    RequestDTOs.Add(GroupRequestDTO);
                }
                var CurrentUserAsRequestUser = RequestDTOs.FirstOrDefault(x => x.Username == CurrentUser.UserName);
                var CurrentUserAsMember = groupMembers.FirstOrDefault(x => x.Username == CurrentUser.UserName);

                var GroupAdmin = _GroupStore.GetGroupMembers(Id)
                .FirstOrDefault(x => x.Type.Equals(GroupUserType.Admin)
                && x.UserId.Equals(CurrentUser.Id));

                var GroupDetailsDTO = new GroupDetailsDTO()
                {
                    Id = existingGroup.Id,
                    Name = existingGroup.Name,
                    Description = existingGroup.Description,
                    CreationDate = existingGroup.CreationDate,
                    GroupOwner = Owner.Username,
                    CurrentUsername = CurrentUser.UserName,
                    GroupRequests = RequestDTOs,
                    PendingRequest = (CurrentUserAsRequestUser != null) ? true : false,
                    ExistingMember = (CurrentUserAsMember != null) ? true : false,
                    Admin = (GroupAdmin != null) ? true : false
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

                var existingGroup = _GroupStore.GetGroupOwner(Id);
                if (CurrentUser.Id.Equals(existingGroup.UserId) is false)
                    throw new ForbiddenException(ErrorMessages.ForbiddenAccess);

                return View(new AddGroupUserDTO { Id = Id });
            }
            catch (ForbiddenException ex)
            {
                _logger.LogError(ex.Message);
                return RedirectToAction(ActionName.NotAuthorized, ControllerName.Accounts);
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
                var Groups = _GroupStore.GetAllGroup();
                var GroupViewModels = new List<GroupResultViewModel>();
                foreach (var group in Groups)
                {
                    GroupViewModels.Add(new GroupResultViewModel()
                    {
                        Groupname = group.Name,
                        GroupId = group.Id,
                        CreatedOn = group.CreationDate,
                        CreatedBy = _GroupStore.GetGroupOwner(group.Id).Username
                    });
                }
                return View(new SearchGroupViewModel()
                {
                    Groups = GroupViewModels
                });
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
                    var GroupViewModel = new List<GroupResultViewModel>();

                    var GroupResult = new GroupResultViewModel()
                    {
                        Groupname = existingGroup.Name,
                        GroupId = existingGroup.Id,
                        CreatedOn = existingGroup.CreationDate,
                        CreatedBy = _GroupStore.GetGroupOwner(existingGroup.Id).Username
                    };
                    GroupViewModel.Add(GroupResult);
                    searchGroupViewModel.Groups = GroupViewModel;
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

                if (!existingGroup.UserId.Equals(CurrentUser.Id))
                    throw new ForbiddenException(ErrorMessages.ForbiddenAccess);

                return View(existingGroup);
            }
            catch (DomainException ex)
            {
                _logger.LogError(ex.Message);
                return RedirectToAction(ActionName.Login, ControllerName.Accounts);
            }
            catch (ForbiddenException ex)
            {
                _logger.LogError(ex.Message);
                return RedirectToAction(ActionName.NotAuthorized, ControllerName.Accounts);
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

                var existingGroup = _GroupStore.GetGroupOwner(existingGroupMember.GroupId);
                if (CurrentUser.Id.Equals(existingGroup.UserId) is false)
                    throw new ForbiddenException(ErrorMessages.ForbiddenAccess);
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
            catch (ForbiddenException ex)
            {
                _logger.LogError(ex.Message);
                return RedirectToAction(ActionName.NotAuthorized, ControllerName.Accounts);
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


        public IActionResult Edit(Guid Id)
        {
            try
            {
                var CurrentUser = _context.Users.Find(userManager.GetUserId(User));
                var existingAccount = _userStore.GetByIdentityUserId(CurrentUser.Id);
                if (existingAccount.AccountStatus.Equals(Status.Suspended))
                    signInManager.SignOutAsync();


                var existingGroup = _GroupStore.GetGroupById(Id);

                var Admin = _GroupStore.GetGroupMembers(existingGroup.Id)
                .FirstOrDefault(x => x.Type.Equals(GroupUserType.Admin) && x.UserId.Equals(CurrentUser.Id));

                if (!CurrentUser.Id.Equals(existingGroup.UserId) && Admin is null)
                    throw new ForbiddenException(ErrorMessages.ForbiddenAccess);

                var editGroupDTO = new EditGroupDTO() { GroupID = new Guid(Id.ToString()) };
                return View(existingGroup);

            }
            catch (DomainException ex)
            {
                _logger.LogError(ex.Message);
                return RedirectToAction(ActionName.NotFound, ControllerName.Accounts);

            }
            catch (ForbiddenException ex)
            {
                _logger.LogError(ex.Message);
                return RedirectToAction(ActionName.NotAuthorized, ControllerName.Accounts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return RedirectToAction(ActionName.ServerError, ControllerName.Accounts);
            }


        }

        [HttpPost]
        public IActionResult Edit(Group Group)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var existingGroup = _GroupStore.GetGroupById(Group.Id);
                    if (String.IsNullOrEmpty(Group.Name))
                        throw new DomainException(ErrorMessages.RequiredValuesNotProvided);

                    existingGroup.Name = Group.Name;
                    existingGroup.Description = Group.Description;
                    _context.SaveChanges();

                    ViewBag.Message = "Success";

                }
                return View(Group);


            }
            catch (DomainException ex)
            {
                _logger.LogError(ex.Message);
                ModelState.AddModelError("Error", ex.Message);
                return View(Group);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return RedirectToAction(ActionName.ServerError, ControllerName.Accounts);
            }
        }
        public IActionResult AddGroupAdmin(Guid Id)
        {
            try
            {

                var CurrentUser = _context.Users.Find(userManager.GetUserId(User));
                var existingAccount = _userStore.GetByIdentityUserId(CurrentUser.Id);
                if (existingAccount.AccountStatus.Equals(Status.Suspended))
                    signInManager.SignOutAsync();


                var existingGroupUser = _GroupStore.GetGroupMemberById(Id);

                var existingGroup = _GroupStore.GetGroupById(existingGroupUser.GroupId);
                if (CurrentUser.Id.Equals(existingGroup.UserId) is false)
                    throw new ForbiddenException(ErrorMessages.ForbiddenAccess);

                var GroupMemberDTO = new GroupMemberDTO()
                {
                    Id = existingGroupUser.Id,
                    Username = existingGroupUser.Username,
                    Type = existingGroupUser.Type,
                    GroupId = existingGroupUser.GroupId,
                    UserId = existingGroupUser.UserId
                };
                return View(GroupMemberDTO);

            }
            catch (DomainException ex)
            {
                _logger.LogError(ex.Message);
                ModelState.AddModelError("Error", ex.Message);
                return View();
            }
            catch (ForbiddenException ex)
            {
                _logger.LogError(ex.Message);
                return RedirectToAction(ActionName.NotAuthorized, ControllerName.Accounts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return RedirectToAction(ActionName.ServerError, ControllerName.Accounts);
            }
        }

        public IActionResult ConfirmGroupAdmin(Guid Id)
        {
            try
            {
                var existingGroupUser = _GroupStore.GetGroupMemberById(Id);
                existingGroupUser.Type = GroupUserType.Admin;
                var GroupID = existingGroupUser.GroupId;
                _context.SaveChanges();
                return RedirectToAction("GroupDetails", "Groups", new { id = GroupID });

            }

            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return RedirectToAction(ActionName.ServerError, ControllerName.Accounts);
            }

        }


        public IActionResult RevokeGroupAdmin(Guid Id)
        {
            try
            {

                var CurrentUser = _context.Users.Find(userManager.GetUserId(User));
                var existingAccount = _userStore.GetByIdentityUserId(CurrentUser.Id);
                if (existingAccount.AccountStatus.Equals(Status.Suspended))
                    signInManager.SignOutAsync();


                var existingGroupUser = _GroupStore.GetGroupMemberById(Id);

                var existingGroup = _GroupStore.GetGroupById(existingGroupUser.GroupId);
                if (CurrentUser.Id.Equals(existingGroup.UserId) is false)
                    throw new ForbiddenException(ErrorMessages.ForbiddenAccess);

                var GroupMemberDTO = new GroupMemberDTO()
                {
                    Id = existingGroupUser.Id,
                    Username = existingGroupUser.Username,
                    Type = existingGroupUser.Type,
                    GroupId = existingGroupUser.GroupId,
                    UserId = existingGroupUser.UserId
                };
                return View(GroupMemberDTO);

            }
            catch (DomainException ex)
            {
                _logger.LogError(ex.Message);
                ModelState.AddModelError("Error", ex.Message);
                return View();
            }
            catch (ForbiddenException ex)
            {
                _logger.LogError(ex.Message);
                return RedirectToAction(ActionName.NotAuthorized, ControllerName.Accounts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return RedirectToAction(ActionName.ServerError, ControllerName.Accounts);
            }
        }

        public IActionResult ConfirmRevoke(Guid Id)
        {
            try
            {
                var existingGroupUser = _GroupStore.GetGroupMemberById(Id);
                existingGroupUser.Type = GroupUserType.Member;
                var GroupID = existingGroupUser.GroupId;
                _context.SaveChanges();
                return RedirectToAction("GroupDetails", "Groups", new { id = GroupID });

            }

            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return RedirectToAction(ActionName.ServerError, ControllerName.Accounts);
            }

        }



        public IActionResult SendRequest(Guid Id)
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

                if (CurrentUser is null)
                    throw new DomainException(ErrorMessages.NotSignedIn);


                var groupMembers = _GroupStore.GetGroupMembers(Id);
                var CurrentUserAsMember = groupMembers.FirstOrDefault(x => x.UserId == CurrentUser.Id);
                if (CurrentUserAsMember != null)
                    throw new DomainException(ErrorMessages.GroupUserExists);

                return View(new GroupDetailsDTO()
                {
                    Id = existingGroup.Id,
                    Name = existingGroup.Name,
                    Description = existingGroup.Description,
                    CurrentUsername = CurrentUser.UserName
                });
            }
            catch (DomainException ex)
            {
                _logger.LogError(ex.Message);
                if (ex.Message.Equals(ErrorMessages.GroupDoesNotExist))
                    return RedirectToAction(ActionName.NotFound, ControllerName.Accounts);
                if (ex.Message.Equals(ErrorMessages.GroupUserExists))
                    return RedirectToAction("AlreadyExists");
                return RedirectToAction(ActionName.ServerError, ControllerName.Accounts);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return RedirectToAction(ActionName.ServerError, ControllerName.Accounts);
            }



        }
        [HttpPost]
        public IActionResult ConfirmSend(Guid Id)
        {
            try
            {
                var CurrentUser = _context.Users.Find(userManager.GetUserId(User));
                var existingGroup = _GroupStore.GetGroupById(Id);
                var Requestor = new GroupRequestUser()
                {
                    Username = CurrentUser.UserName,
                    UserId = CurrentUser.Id,
                };
                _GroupRequestStore.NewRequestUser(Requestor);

                var Request = new GroupRequest()
                {
                    GroupId = Id,
                    RequestDate = DateTime.Now,
                    RequestUserId = Requestor.Id
                };
                _GroupRequestStore.SendRequest(Request);

                return RedirectToAction("GroupDetails", "Groups", new { id = Request.GroupId });
            }
            catch (DomainException ex)
            {
                _logger.LogError(ex.Message);
                if (ex.Message.Equals(ErrorMessages.GroupDoesNotExist))
                    return RedirectToAction(ActionName.NotFound, ControllerName.Accounts);
                if (ex.Message.Equals(ErrorMessages.GroupUserExists))
                    return RedirectToAction("AlreadyExists");
                return RedirectToAction(ActionName.ServerError, ControllerName.Accounts);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return RedirectToAction(ActionName.ServerError, ControllerName.Accounts);
            }
        }

        public IActionResult AcceptRequest(Guid Id)
        {

            try
            {
                var existingRequest = _GroupRequestStore.GetGroupRequest(Id);
                if (existingRequest is null)
                    throw new DomainException(ErrorMessages.RequestDoesNotExist);

                var CurrentUser = _context.Users.Find(userManager.GetUserId(User));
                if (CurrentUser is null)
                    throw new DomainException(ErrorMessages.NotSignedIn);

                var existingAccount = _userStore.GetByIdentityUserId(CurrentUser.Id);
                if (existingAccount.AccountStatus.Equals(Status.Suspended))
                    signInManager.SignOutAsync();

                var GroupOwner = _GroupStore.GetGroupOwner(existingRequest.GroupId);
                var GroupID = GroupOwner.GroupId;
                var Admin = _GroupStore.GetGroupMembers(GroupID)
               .FirstOrDefault(x => x.Type.Equals(GroupUserType.Admin) && x.UserId.Equals(CurrentUser.Id));

                if (!CurrentUser.Id.Equals(GroupOwner.UserId) && Admin is null)
                    throw new ForbiddenException(ErrorMessages.ForbiddenAccess);


                var existingRequestUser = _GroupRequestStore.GetGroupRequestUser(existingRequest.RequestUserId);
                var GroupUser = new GroupUser()
                {
                    Username = existingRequestUser.Username,
                    UserId = existingRequestUser.UserId,
                    GroupId = existingRequest.GroupId,
                    Type = GroupUserType.Member
                };

                _GroupStore.CreateGroupUser(GroupUser);
                _GroupRequestStore.DeleteRequest(existingRequest.Id);
                _GroupRequestStore.DeleteRequestUser(existingRequestUser.Id);
                return RedirectToAction("GroupDetails", "Groups", new { id = GroupID });
            }
            catch (DomainException ex)
            {
                _logger.LogError(ex.Message);
                if (ex.Message.Equals(ErrorMessages.RequestDoesNotExist))
                    return RedirectToAction(ActionName.NotFound, ControllerName.Accounts);

                return RedirectToAction(ActionName.Login, ControllerName.Accounts);
            }
            catch (ForbiddenException ex)
            {
                _logger.LogError(ex.Message);
                return RedirectToAction(ActionName.NotAuthorized, ControllerName.Accounts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return RedirectToAction(ActionName.ServerError, ControllerName.Accounts);
            }
        }


        public IActionResult DeclineRequest(Guid Id)
        {

            try
            {
                var existingRequest = _GroupRequestStore.GetGroupRequest(Id);
                if (existingRequest is null)
                    throw new DomainException(ErrorMessages.RequestDoesNotExist);

                var CurrentUser = _context.Users.Find(userManager.GetUserId(User));
                if (CurrentUser is null)
                    throw new DomainException(ErrorMessages.NotSignedIn);

                var existingAccount = _userStore.GetByIdentityUserId(CurrentUser.Id);
                if (existingAccount.AccountStatus.Equals(Status.Suspended))
                    signInManager.SignOutAsync();

                var GroupOwner = _GroupStore.GetGroupOwner(existingRequest.GroupId);
                var GroupID = GroupOwner.GroupId;
                var Admin = _GroupStore.GetGroupMembers(GroupID)
               .FirstOrDefault(x => x.Type.Equals(GroupUserType.Admin) && x.UserId.Equals(CurrentUser.Id));

                if (!CurrentUser.Id.Equals(GroupOwner.UserId) && Admin is null)
                    throw new ForbiddenException(ErrorMessages.ForbiddenAccess);


                var existingRequestUser = _GroupRequestStore.GetGroupRequestUser(existingRequest.RequestUserId);

                _GroupRequestStore.DeleteRequest(existingRequest.Id);
                _GroupRequestStore.DeleteRequestUser(existingRequestUser.Id);
                return RedirectToAction("GroupDetails", "Groups", new { id = GroupID });
            }
            catch (DomainException ex)
            {
                _logger.LogError(ex.Message);
                if (ex.Message.Equals(ErrorMessages.RequestDoesNotExist))
                    return RedirectToAction(ActionName.NotFound, ControllerName.Accounts);

                return RedirectToAction(ActionName.Login, ControllerName.Accounts);
            }
            catch (ForbiddenException ex)
            {
                _logger.LogError(ex.Message);
                return RedirectToAction(ActionName.NotAuthorized, ControllerName.Accounts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return RedirectToAction(ActionName.ServerError, ControllerName.Accounts);
            }
        }





        public IActionResult PendingRequests(Guid Id)
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

                var GroupOwner = _GroupStore.GetGroupOwner(existingGroup.Id);

                var Admin = _GroupStore.GetGroupMembers(Id)
                .FirstOrDefault(x => x.Type.Equals(GroupUserType.Admin)
                && x.UserId.Equals(CurrentUser.Id));

                if (!CurrentUser.Id.Equals(GroupOwner.UserId) && Admin is null)
                    throw new ForbiddenException(ErrorMessages.ForbiddenAccess);

                var GroupRequests = _GroupRequestStore.GetGroupRequestsOfGroup(existingGroup.Id);

                var GroupRequestViewModel = new GroupRequestsViewModel()
                {
                    GroupOwner = GroupOwner.Username,
                    OwnerIdentityUserID = GroupOwner.UserId,
                    GroupID = existingGroup.Id,
                    requests = new List<GroupRequestDTO>()
                };
                foreach (var Request in GroupRequests)
                {
                    var RequestUser = _GroupRequestStore.GetGroupRequestUser(Request.RequestUserId);
                    var GroupRequestDTO = new GroupRequestDTO()
                    {
                        Id = Request.Id,
                        Username = RequestUser.Username,
                        RequestDate = Request.RequestDate,
                        Pending = Request.Pending,
                        Message = $"{RequestUser.Username} would like to join your group."

                    };
                    GroupRequestViewModel.requests.Add(GroupRequestDTO);
                }

                return View(GroupRequestViewModel);
            }
            catch (DomainException ex)
            {
                _logger.LogError(ex.Message);
                if (ex.Message.Equals(ErrorMessages.GroupDoesNotExist))
                    return RedirectToAction(ActionName.NotFound, ControllerName.Accounts);
                return RedirectToAction(ActionName.Login, ControllerName.Accounts);
            }
            catch (ForbiddenException ex)
            {
                _logger.LogError(ex.Message);
                return RedirectToAction(ActionName.NotAuthorized, ControllerName.Accounts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return RedirectToAction(ActionName.ServerError, ControllerName.Accounts);
            }
        }

        [HttpGet]
        public IActionResult LeaveGroup(Guid Id)
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

                var existingGroupMember = _GroupStore.GetGroupMembers(Id)
                .FirstOrDefault(x => x.UserId.Equals(CurrentUser.Id));

                if (existingGroupMember is null)
                    throw new DomainException(ErrorMessages.UserDoesNotExist);

                var GroupID = existingGroupMember.GroupId.ToString().Clone();


                var GroupMemberDTO = new GroupMemberDTO()
                {
                    GroupId = existingGroup.Id,
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
            catch (ForbiddenException ex)
            {
                _logger.LogError(ex.Message);
                return RedirectToAction(ActionName.NotAuthorized, ControllerName.Accounts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return RedirectToAction(ActionName.ServerError, ControllerName.Accounts);
            }
        }
        [HttpPost]
        public IActionResult ConfirmLeave(Guid Id)
        {
            try
            {
                var CurrentUser = _context.Users.Find(userManager.GetUserId(User));

                var existingGroup = _GroupStore.GetGroupById(Id);
                var existingGroupMember = _GroupStore.GetGroupMembers(existingGroup.Id)
                .FirstOrDefault(x => x.UserId.Equals(CurrentUser.Id));

                _GroupStore.DeleteGroupUser(existingGroupMember);
                return RedirectToAction(ActionName.Profile, ControllerName.Accounts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return RedirectToAction(ActionName.ServerError, ControllerName.Accounts);
            }


        }
    }
}