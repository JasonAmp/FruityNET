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

namespace FruityNET.Controllers
{
    public class GroupsController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        private readonly ApplicationDbContext _context;
        private readonly IUserStore _userStore;
        private readonly IFriendsListStore _FriendListStore;
        private readonly IGroupStore _GroupStore;

        public GroupsController(UserManager<User> userManager, SignInManager<User> signInManager, ApplicationDbContext _context,
        IUserStore _userStore, IFriendsListStore _FriendListStore, IGroupStore _GroupStore)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this._context = _context;
            this._userStore = _userStore;
            this._GroupStore = _GroupStore;
        }

        [HttpGet]
        public IActionResult AddGroup()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddGroup(AddGroupDTO AddGroupDTO)
        {
            if (ModelState.IsValid)
            {
                var CurrentUser = _context.Users.Find(userManager.GetUserId(User));
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

        [HttpGet]
        public IActionResult GroupDetails(Guid Id)
        {
            var existingGroup = _GroupStore.GetGroupById(Id);
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


        public IActionResult AddGroupMember(Guid Id)
        {

            return View(new AddGroupUserDTO { Id = Id });

        }


        [HttpPost]
        public IActionResult AddGroupMember(AddGroupUserDTO addGroupUserDTO)
        {
            if (String.IsNullOrEmpty(addGroupUserDTO.Username))
                ModelState.AddModelError("Error", "Please provide a Username");

            if (ModelState.IsValid)
            {
                var existingUser = _userStore.GetByUsername(addGroupUserDTO.Username);
                if (existingUser is null)
                    ModelState.AddModelError("Error", "User Does Not Exist.");


                var existingGroupMember = (existingUser != null) ? _GroupStore.GetGroupMembers(addGroupUserDTO.Id)
                .FirstOrDefault(x => x.Username == addGroupUserDTO.Username) : null;

                if (existingGroupMember != null)
                    ModelState.AddModelError("Error", "User is already part of Group.");

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
        [HttpGet]
        public IActionResult SearchGroup()
        {
            var CurrentUser = _context.Users.Find(userManager.GetUserId(User));
            if (CurrentUser is null)
                return RedirectToAction("Login", "Accounts");

            return View(new SearchGroupViewModel());

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
            var existingGroup = _GroupStore.GetGroupById(Id);
            return View(existingGroup);

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
            return RedirectToAction("Profile", "Accounts");

        }


        [HttpGet]
        public IActionResult RemoveUser(Guid Id)
        {
            var existingGroupMember = _GroupStore.GetGroupMemberById(Id);
            var GroupID = existingGroupMember.GroupId.ToString().Clone();
            var GroupMemberDTO = new GroupMemberDTO()
            {
                GroupId = new Guid(GroupID.ToString()),
                Id = existingGroupMember.Id,
                Username = existingGroupMember.Username
            };
            return View(GroupMemberDTO);

        }
        [HttpPost]
        public IActionResult RemoveUser(GroupMemberDTO GroupMemberDTO)
        {
            if (ModelState.IsValid)
            {

                _GroupStore.DeleteGroupUser(_GroupStore.GetGroupMemberById(GroupMemberDTO.Id));
            }
            return RedirectToAction("Profile", "Accounts");

        }




    }
}