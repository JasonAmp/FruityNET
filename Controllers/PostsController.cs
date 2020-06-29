using System;
using FruityNET.Data;
using FruityNET.DTOs;
using FruityNET.Entities;
using FruityNET.EntityStore;
using FruityNET.IEntityStore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using FruityNET.ParameterStrings;
using FruityNET.Enums;
using FruityNET.Exceptions;

namespace FruityNET.Controllers
{
    public partial class PostsController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        private readonly ApplicationDbContext _context;
        private readonly IUserStore _userStore;
        private readonly IPostStore _postStore;
        private readonly ICommentStore _commentStore;
        private readonly CurrentPostDTO _currentpost;
        private readonly IFriendsListStore _FriendListStore;
        private readonly INotificationBox _notificationBox;
        private readonly ILogger<PostsController> _logger;





        public PostsController(UserManager<User> userManager, SignInManager<User> signInManager, ApplicationDbContext _context,
        IUserStore _userStore, IPostStore _postStore, ICommentStore _commentStore, CurrentPostDTO _currentpost,
        IFriendsListStore _FriendListStore, INotificationBox _notificationBox, ILogger<PostsController> _logger)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this._context = _context;
            this._userStore = _userStore;
            this._postStore = _postStore;
            this._commentStore = _commentStore;
            this._currentpost = _currentpost;
            this._FriendListStore = _FriendListStore;
            this._notificationBox = _notificationBox;
            this._logger = _logger;
        }

        [HttpGet]
        public IActionResult Post()
        {
            try
            {
                var CurrentUser = _context.Users.Find(userManager.GetUserId(User));
                if (CurrentUser is null)
                    return RedirectToAction("Login", "Accounts");
                var existingAccount = _userStore.GetByIdentityUserId(CurrentUser.Id);
                if (existingAccount.AccountStatus.Equals(Status.Suspended))
                    signInManager.SignOutAsync();

                return View(new AddPostDTO
                {
                    UserId = existingAccount.Id

                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return RedirectToAction(ActionName.ServerError, ControllerName.Accounts);

            }


        }

        [HttpPost]
        public IActionResult Post(AddPostDTO addPostDTO)
        {
            try
            {
                var CurrentUser = _context.Users.Find(userManager.GetUserId(User));
                var existingAccount = _userStore.GetByIdentityUserId(CurrentUser.Id);
                if (existingAccount.AccountStatus.Equals(Status.Suspended))
                    signInManager.SignOutAsync();

                if (ModelState.IsValid)
                {
                    var Post = new Post
                    {
                        Content = addPostDTO.Content,
                        UserId = CurrentUser.Id

                    };
                    _postStore.AddPost(Post);
                    existingAccount.Posts.Add(Post);
                    return RedirectToAction("AllPosts");
                }
                return View(addPostDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return RedirectToAction(ActionName.ServerError, ControllerName.Accounts);

            }



        }


        [HttpGet]
        public IActionResult AllPosts()
        {
            try
            {
                var CurrentUser = _context.Users.Find(userManager.GetUserId(User));
                if (CurrentUser is null)
                    return RedirectToAction("Login", "Accounts");

                var existingAccount = _userStore.GetByIdentityUserId(CurrentUser.Id);
                if (existingAccount.AccountStatus.Equals(Status.Suspended))
                    signInManager.SignOutAsync();

                var friendList = _FriendListStore.GetFriendListOfUser(CurrentUser.Id);
                friendList.Users = _FriendListStore.GetFriendsOfUser(friendList.Id);

                var postViewDTO = new PostViewDto
                {
                    Permissions = existingAccount.UserType,
                    AllPosts = new List<PostDTO>()
                };
                foreach (var friend in friendList.Users)
                {
                    var FriendAccount = _userStore.GetByIdentityUserId(friend.UserId);
                    if (FriendAccount.AccountStatus.Equals(Status.Active))
                    {
                        var AllPosts = _postStore.AllPostByUser(friend.UserId);
                        foreach (var post in AllPosts)
                        {
                            var PostDTO = new PostDTO
                            {
                                Id = post.Id,
                                Content = post.Content,
                                DatePosted = post.DatePosted,
                                Username = friend.Username,
                                UserId = FriendAccount.Id

                            };
                            postViewDTO.AllPosts.Add(PostDTO);
                        }
                    }

                }

                foreach (var post in _postStore.AllPostByCurrentUser(CurrentUser.Id))
                {
                    var PostDTO = new PostDTO
                    {
                        Id = post.Id,
                        Content = post.Content,
                        DatePosted = post.DatePosted,
                        Username = existingAccount.Username,
                        UserId = existingAccount.Id,
                        IdentityId = CurrentUser.Id
                    };
                    postViewDTO.AllPosts.Add(PostDTO);
                }
                return View(postViewDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return RedirectToAction(ActionName.ServerError, ControllerName.Accounts);

            }
        }

        [HttpGet]
        public IActionResult ViewPost(Guid Id)
        {
            try
            {
                var CurrentUser = _context.Users.Find(userManager.GetUserId(User));
                if (CurrentUser is null)
                    return RedirectToAction("Login", "Accounts");

                var existingAccount = _userStore.GetByIdentityUserId(CurrentUser.Id);
                if (existingAccount.AccountStatus.Equals(Status.Suspended))
                    signInManager.SignOutAsync();

                var post = _postStore.ViewPost(Id);

                if (post is null)
                    return RedirectToAction("NotFound", "Accounts");

                var OwnerOfPost = _userStore.GetByIdentityUserId(post.UserId);

                var comments = from x in _commentStore.GetAllComments()
                               where x.PostId == post.Id
                               orderby x.DatePosted
                               select x;
                var ListOfComments = new List<ViewCommentDTO>();
                foreach (var comment in comments)
                {
                    var OwnerOfComment = _commentStore.GetOwnerOfComment(comment.UserId);
                    var CommentViewDTO = new ViewCommentDTO
                    {
                        UserId = comment.UserId,
                        PostId = comment.PostId,
                        Content = comment.Content,
                        Username = OwnerOfComment.Username,
                        DatePosted = comment.DatePosted
                    };
                    ListOfComments.Add(CommentViewDTO);
                }

                return View(new PostViewDetaisDTO
                {
                    Username = OwnerOfPost.Username,
                    content = post.Content,
                    DatePosted = post.DatePosted,
                    comments = ListOfComments
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return RedirectToAction(ActionName.ServerError, ControllerName.Accounts);

            }


        }

        [HttpGet]
        public IActionResult Edit(Guid Id)
        {
            try
            {
                var CurrentUser = _context.Users.Find(userManager.GetUserId(User));
                if (CurrentUser is null)
                    return RedirectToAction("Login", "Accounts");

                var existingAccount = _userStore.GetByIdentityUserId(CurrentUser.Id);
                if (existingAccount.AccountStatus.Equals(Status.Suspended))
                    signInManager.SignOutAsync();

                var existingPost = _postStore.GetById(Id);
                if (existingPost is null)
                    return RedirectToAction("NotFound", "Accounts");


                return View(new EditPostDTO()
                {
                    Id = existingPost.Id,
                    Content = existingPost.Content
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return RedirectToAction(ActionName.ServerError, ControllerName.Accounts);
            }


        }

        [HttpPost]
        public IActionResult Edit(EditPostDTO editPostDTO)
        {
            try
            {
                var existingPost = _postStore.GetById(editPostDTO.Id);


                if (ModelState.IsValid)
                {
                    _postStore.EditPost(editPostDTO, existingPost);
                    return RedirectToAction("AllPosts");
                }
                return View(editPostDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return RedirectToAction(ActionName.ServerError, ControllerName.Accounts);
            }


        }

        [HttpGet]
        public IActionResult Delete(Guid Id)
        {
            try
            {
                var CurrentUser = _context.Users.Find(userManager.GetUserId(User));
                var existingAccount = _userStore.GetByIdentityUserId(CurrentUser.Id);

                if (CurrentUser is null)
                    throw new DomainException(ErrorMessages.NotSignedIn);


                if (existingAccount.AccountStatus.Equals(Status.Suspended))
                    signInManager.SignOutAsync();


                var existingPost = _postStore.GetById(Id);
                if (existingPost is null)
                    throw new DomainException(ErrorMessages.PostDoesNotExist);

                return View(existingPost);
            }
            catch (DomainException ex)
            {
                _logger.LogError(ex.Message);
                if (ex.Message.Equals(ErrorMessages.PostDoesNotExist))
                    return RedirectToAction(ActionName.NotFound, ControllerName.Accounts);

                return RedirectToAction(ActionName.Login, ControllerName.Accounts);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return RedirectToAction(ActionName.ServerError, ControllerName.Accounts);

            }


        }

        [HttpPost]
        public IActionResult Delete(Post Post)
        {
            try
            {
                var CurrentUser = _context.Users.Find(userManager.GetUserId(User));
                var existingAccount = _userStore.GetByIdentityUserId(CurrentUser.Id);

                if (existingAccount.AccountStatus.Equals(Status.Suspended))
                    signInManager.SignOutAsync();

                var existingPost = _postStore.GetById(Post.Id);


                if (existingPost.UserId != CurrentUser.Id)
                {
                    var Notification = new Notification()
                    {
                        Message = $"Your post '{existingPost.Content}' was deleted," + " " + "It may have violated community guidlines.",
                        NotificationBoxId = _notificationBox.GetNotificationBoxByUserId(existingPost.UserId).Id,
                        RecieverUsername = _userStore.GetByIdentityUserId(existingPost.UserId).Username
                    };


                    _notificationBox.SendNotifcation(Notification);
                    _context.SaveChanges();
                }
                _postStore.DeletePost(existingPost);
                return RedirectToAction("AllPosts");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return RedirectToAction(ActionName.ServerError, ControllerName.Accounts);

            }


        }

        [HttpGet]
        public IActionResult AddCommentToPost(Guid Id)
        {
            try
            {
                var CurrentUser = _context.Users.Find(userManager.GetUserId(User));
                if (CurrentUser is null)
                    return RedirectToAction("Login", "Accounts");
                var existingAccount = _userStore.GetByIdentityUserId(CurrentUser.Id);
                if (existingAccount.AccountStatus.Equals(Status.Suspended))
                    signInManager.SignOutAsync();
                _currentpost.Id = Id;

                var existingPost = _postStore.ViewPost(Id);
                var addCommentDTO = new AddCommentDTO
                {
                    UserId = CurrentUser.Id,
                    PostId = existingPost.Id
                };
                return View(addCommentDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return RedirectToAction(ActionName.ServerError, ControllerName.Accounts);

            }



        }

        [HttpPost]
        public IActionResult AddCommentToPost(AddCommentDTO addCommentDTO)
        {
            try
            {
                var CurrentUser = _context.Users.Find(userManager.GetUserId(User));
                if (CurrentUser is null)
                    return RedirectToAction("Login", "Accounts");
                var existingAccount = _userStore.GetByIdentityUserId(CurrentUser.Id);


                if (ModelState.IsValid)
                {
                    var existingPost = _postStore.ViewPost(_currentpost.Id);
                    if (existingPost is null)
                        return RedirectToAction("NotFound", "Accounts");

                    var comment = new Comment()
                    {
                        Content = addCommentDTO.Content,
                        PostId = _currentpost.Id,
                        UserId = CurrentUser.Id,
                        DatePosted = DateTime.Now
                    };

                    existingPost.Comments.Add(comment);
                    _commentStore.AddComment(comment);
                    if (existingPost.UserId != CurrentUser.Id)
                    {
                        var Notification = new Notification()
                        {
                            Message = $"{CurrentUser.UserName} Has Commented on your Post '{existingPost.Content}' ",
                            NotificationBoxId = _notificationBox.GetNotificationBoxByUserId(existingPost.UserId).Id,
                            RecieverUsername = _userStore.GetByIdentityUserId(existingPost.UserId).Username
                        };


                        _notificationBox.SendNotifcation(Notification);
                    }

                    _context.SaveChanges();
                    return RedirectToAction("ViewPost", "Posts", new { id = existingPost.Id });
                }
                else
                {
                    return View(addCommentDTO);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return RedirectToAction(ActionName.ServerError, ControllerName.Accounts);

            }


        }



    }
}