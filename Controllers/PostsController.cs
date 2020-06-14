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



        public PostsController(UserManager<User> userManager, SignInManager<User> signInManager, ApplicationDbContext _context,
        IUserStore _userStore, IPostStore _postStore, ICommentStore _commentStore, CurrentPostDTO _currentpost,
        IFriendsListStore _FriendListStore)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this._context = _context;
            this._userStore = _userStore;
            this._postStore = _postStore;
            this._commentStore = _commentStore;
            this._currentpost = _currentpost;
            this._FriendListStore = _FriendListStore;
        }

        [HttpGet]
        public IActionResult Post()
        {
            var CurrentUser = _context.Users.Find(userManager.GetUserId(User));
            if (CurrentUser is null)
                return RedirectToAction("Login", "Accounts");
            var existingAccount = _userStore.GetByIdentityUserId(CurrentUser.Id);

            return View(new AddPostDTO
            {
                UserId = existingAccount.Id

            });
        }

        [HttpPost]
        public IActionResult Post(AddPostDTO addPostDTO)
        {
            var CurrentUser = _context.Users.Find(userManager.GetUserId(User));
            var existingAccount = _userStore.GetByIdentityUserId(CurrentUser.Id);

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


        [HttpGet]
        public IActionResult AllPosts()
        {
            var CurrentUser = _context.Users.Find(userManager.GetUserId(User));
            if (CurrentUser is null)
                return RedirectToAction("Login", "Accounts");

            var existingAccount = _userStore.GetByIdentityUserId(CurrentUser.Id);
            var friendList = _FriendListStore.GetFriendListOfUser(CurrentUser.Id);
            friendList.Users = _FriendListStore.GetFriendsOfUser(friendList.Id);

            var postViewDTO = new PostViewDto { AllPosts = new List<PostDTO>() };
            foreach (var friend in friendList.Users)
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
                    };
                    postViewDTO.AllPosts.Add(PostDTO);
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

        [HttpGet]
        public IActionResult ViewPost(Guid Id)
        {
            var CurrentUser = _context.Users.Find(userManager.GetUserId(User));
            if (CurrentUser is null)
                return RedirectToAction("Login", "Accounts");

            var existingAccount = _userStore.GetByIdentityUserId(CurrentUser.Id);


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
                    Username = OwnerOfComment.Username
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

        [HttpGet]
        public IActionResult Edit(Guid Id)
        {
            var CurrentUser = _context.Users.Find(userManager.GetUserId(User));
            if (CurrentUser is null)
                return RedirectToAction("Login", "Accounts");

            var existingAccount = _userStore.GetByIdentityUserId(CurrentUser.Id);

            var existingPost = _postStore.GetById(Id);
            if (existingPost is null)
                return RedirectToAction("NotFound", "Accounts");


            return View(new EditPostDTO()
            {
                Id = existingPost.Id,
                Content = existingPost.Content
            });
        }

        [HttpPost]
        public IActionResult Edit(EditPostDTO editPostDTO)
        {
            var existingPost = _postStore.GetById(editPostDTO.Id);


            if (ModelState.IsValid)
            {
                _postStore.EditPost(editPostDTO, existingPost);
                return RedirectToAction("AllPosts");
            }
            return View(editPostDTO);
        }

        public IActionResult Delete(Guid Id)
        {
            var existingPost = _postStore.GetById(Id);
            if (existingPost is null)
                return RedirectToAction("NotFound", "Accounts");

            _postStore.DeletePost(existingPost);
            return RedirectToAction("AllPosts");
        }

        [HttpGet]
        public IActionResult AddCommentToPost(Guid Id)
        {
            var CurrentUser = _context.Users.Find(userManager.GetUserId(User));
            if (CurrentUser is null)
                return RedirectToAction("Login", "Accounts");
            var existingAccount = _userStore.GetByIdentityUserId(CurrentUser.Id);
            _currentpost.Id = Id;

            var existingPost = _postStore.ViewPost(Id);
            var addCommentDTO = new AddCommentDTO
            {

                UserId = CurrentUser.Id,

            };

            return View(addCommentDTO);



        }

        [HttpPost]
        public IActionResult AddCommentToPost(AddCommentDTO addCommentDTO)
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
                    UserId = CurrentUser.Id
                };

                existingPost.Comments.Add(comment);

                _commentStore.AddComment(comment);



                return RedirectToAction("AllPosts", "Posts");

            }
            else
            {
                return View(addCommentDTO);
            }




        }



    }
}