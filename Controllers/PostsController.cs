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
using FruityNET.Queries;

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

                var GetAllPostsQuery = new GetAllPostsQuery(CurrentUser, existingAccount, _FriendListStore,
                _notificationBox, _userStore, _postStore);

                return View(GetAllPostsQuery.Handle());
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
                    throw new DomainException(ErrorMessages.NotSignedIn);

                var existingAccount = _userStore.GetByIdentityUserId(CurrentUser.Id);
                if (existingAccount.AccountStatus.Equals(Status.Suspended))
                    signInManager.SignOutAsync();

                var existingPost = _postStore.ViewPost(Id);

                if (existingPost is null)
                    throw new DomainException(ErrorMessages.PostDoesNotExist);

                var OwnerOfPost = _userStore.GetByIdentityUserId(existingPost.UserId);

                GetPostDetailsQuery GetPostDetailsQuery = new GetPostDetailsQuery(CurrentUser, OwnerOfPost,
                existingPost, _userStore, _commentStore, existingAccount);

                return View(GetPostDetailsQuery.Handle());
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

                if (!CurrentUser.Id.Equals(existingPost.UserId) && existingAccount.UserType.Equals(UserType.User))
                    throw new ForbiddenException(ErrorMessages.ForbiddenAccess);

                return View(existingPost);
            }
            catch (DomainException ex)
            {
                _logger.LogError(ex.Message);
                if (ex.Message.Equals(ErrorMessages.PostDoesNotExist))
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

        [HttpGet]
        public IActionResult DeleteComment(Guid Id)
        {
            try
            {
                var CurrentUser = _context.Users.Find(userManager.GetUserId(User));
                if (CurrentUser is null)
                    throw new DomainException(ErrorMessages.NotSignedIn);

                var existingComment = _commentStore.GetAllComments().First(x => x.Id == Id);
                var CommentOwner = _commentStore.GetOwnerOfComment(existingComment.UserId);
                if (!CommentOwner.UserId.Equals(CurrentUser.Id))
                    throw new ForbiddenException(ErrorMessages.ForbiddenAccess);
                var existingAccount = _userStore.GetByIdentityUserId(CommentOwner.UserId);

                var DeleteCommentDTO = new DeleteCommentDTO()
                {
                    UserId = CommentOwner.UserId,
                    PostID = existingComment.PostId,
                    PostedByUsername = existingAccount.Username,
                    CommentID = existingComment.Id,
                    Content = existingComment.Content
                };

                return View(DeleteCommentDTO);
            }
            catch (ForbiddenException ex)
            {
                _logger.LogError(ex.Message);
                return RedirectToAction(ActionName.NotAuthorized, ControllerName.Accounts);
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
        public IActionResult ConfirmDelete(Guid Id)
        {
            try
            {
                var CurrentUser = _context.Users.Find(userManager.GetUserId(User));
                if (CurrentUser is null)
                    throw new DomainException(ErrorMessages.NotSignedIn);

                var existingComment = _commentStore.GetAllComments().First(x => x.Id == Id);
                if (existingComment is null)
                    throw new DomainException(ErrorMessages.CommentDoesNotExist);

                var CommentOwner = _commentStore.GetOwnerOfComment(existingComment.UserId);
                if (!CommentOwner.UserId.Equals(CurrentUser.Id))
                    throw new ForbiddenException(ErrorMessages.ForbiddenAccess);

                var DeleteCommentDTO = new DeleteCommentDTO()
                {
                    UserId = CommentOwner.UserId,
                    PostID = existingComment.PostId,
                    CommentID = existingComment.Id,
                    Content = existingComment.Content
                };
                var PostID = new Guid(DeleteCommentDTO.PostID.ToString());

                _commentStore.DeleteComment(existingComment);
                _context.SaveChanges();

                return RedirectToAction("ViewPost", "Posts", new { id = PostID });
            }
            catch (DomainException ex)
            {
                _logger.LogError(ex.Message);
                if (ex.Message.Equals(ErrorMessages.NotSignedIn))
                    return RedirectToAction("Login", "Accounts");

                return RedirectToAction("NotFound", "Accounts");
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



    }
}