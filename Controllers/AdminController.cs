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
    public class AdminController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        private readonly ApplicationDbContext _context;
        private readonly IUserStore _userStore;
        private readonly INotificationBox _notificationBox;
        private readonly ILogger<AccountsController> _logger;
        private readonly IAdminRequestStore _AdminRequestStore;




        public AdminController(UserManager<User> userManager, SignInManager<User> signInManager, ApplicationDbContext _context, IAdminRequestStore _AdminRequestStore,
         INotificationBox _notificationBox, ILogger<AccountsController> _logger, IUserStore _userStore)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this._context = _context;
            this._AdminRequestStore = _AdminRequestStore;
            this._logger = _logger;
            this._notificationBox = _notificationBox;
            this._userStore = _userStore;
        }


        [HttpGet]
        public IActionResult Requests()
        {
            try
            {
                var CurrentUser = _context.Users.Find(userManager.GetUserId(User));
                if (CurrentUser is null)
                    throw new DomainException(ErrorMessages.NotSignedIn);

                var existingAccount = _userStore.GetByUsername(CurrentUser.UserName);


                if (!existingAccount.UserType.Equals(UserType.SiteOwner))
                    throw new ForbiddenException(ErrorMessages.ForbiddenAccess);

                var AdminRequests = _AdminRequestStore.GetAll();
                var AdminRequestModel = new AdminRequestsViewModel() { };
                foreach (var Request in AdminRequests)
                {
                    var Requestor = _AdminRequestStore.GetUserById(Request.AdminRequestorId);
                    AdminRequestModel.AdminRequests.Add(new AdminRequestDTO()
                    {
                        RequestDate = Request.RequestDate,
                        RequestId = Request.Id,
                        Username = Requestor.Username,

                    });
                }

                return View(AdminRequestModel);

            }
            catch (ForbiddenException ex)
            {
                _logger.LogError(ex.Message);
                return RedirectToAction("NotAuthorized", "Accounts");
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
        public IActionResult SendRequest()
        {
            try
            {
                var CurrentUser = _context.Users.Find(userManager.GetUserId(User));
                if (CurrentUser is null)
                    throw new DomainException(ErrorMessages.NotSignedIn);


                return View();

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

        [HttpPost]
        public IActionResult ConfirmSend()
        {
            var CurrentUser = _context.Users.Find(userManager.GetUserId(User));
            var SiteOwner = _AdminRequestStore.GetSiteOwner();
            var AdminRequestBox = _AdminRequestStore.GetAdminBox(SiteOwner.Id);
            var RequestUser = new AdminRequestor()
            {
                Username = CurrentUser.UserName,
                UserId = CurrentUser.Id,

            };
            _AdminRequestStore.AddRequestor(RequestUser);
            var Request = new AdminRequest()
            {
                Username = CurrentUser.UserName,
                RequestDate = DateTime.Now,
                ApprovalBoxId = AdminRequestBox.Id,
                AdminRequestorId = RequestUser.Id
            };
            _AdminRequestStore.AddRequest(Request);
            _context.SaveChanges();
            return RedirectToAction(ActionName.Profile, ControllerName.Accounts);
        }




        [HttpGet]
        public IActionResult ApproveAdmin(Guid Id)
        {
            try
            {
                var CurrentUser = _context.Users.Find(userManager.GetUserId(User));
                if (CurrentUser is null)
                    throw new DomainException(ErrorMessages.NotSignedIn);

                var CurrentUserAccount = _userStore.GetByUsername(CurrentUser.UserName);
                if (!CurrentUserAccount.UserType.Equals(UserType.SiteOwner))
                    throw new ForbiddenException(ErrorMessages.ForbiddenAccess);

                var existingRequest = _AdminRequestStore.GetRequestById(Id);
                var existingRequestUser = _AdminRequestStore.GetUserById(existingRequest.AdminRequestorId);
                var AdminRequestDTO = new AdminRequestDTO()
                {
                    RequestId = existingRequest.Id,
                    RequestDate = existingRequest.RequestDate,
                    RequestorID = existingRequestUser.Id,
                    Username = existingRequestUser.Username,
                    AccountID = existingRequestUser.UserId
                };
                return View(AdminRequestDTO);

            }
            catch (ForbiddenException ex)
            {
                _logger.LogError(ex.Message);
                return RedirectToAction(ActionName.NotAuthorized, ControllerName.Accounts);
            }
            catch (DomainException ex)
            {
                _logger.LogError(ex.Message);
                return RedirectToAction("Login", "Accounts");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return RedirectToAction("ServerError", "Accounts");
            }
        }

        [HttpPost]
        public IActionResult Confirm(Guid Id)
        {
            try
            {
                var existingRequest = _AdminRequestStore.GetRequestById(Id);
                var existingRequestUser = _AdminRequestStore.GetUserById(existingRequest.AdminRequestorId);
                var existingAccount = _userStore.GetByIdentityUserId(existingRequestUser.UserId);
                existingAccount.UserType = UserType.Admin;
                _context.SaveChanges();
                _AdminRequestStore.DeleteRequestor(existingRequestUser.Id);
                return RedirectToAction("AdminPortal", "Accounts");


            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return RedirectToAction("ServerError", "Accounts");
            }
        }

        [HttpPost]
        public IActionResult Reject(Guid Id)
        {
            try
            {
                var CurrentUser = _context.Users.Find(userManager.GetUserId(User));
                if (CurrentUser is null)
                    throw new DomainException(ErrorMessages.NotSignedIn);

                var CurrentUserAccount = _userStore.GetByUsername(CurrentUser.UserName);
                if (!CurrentUserAccount.UserType.Equals(UserType.SiteOwner))
                    throw new ForbiddenException(ErrorMessages.ForbiddenAccess);

                var existingRequest = _AdminRequestStore.GetRequestById(Id);
                var existingRequestUser = _AdminRequestStore.GetUserById(existingRequest.AdminRequestorId);
                _AdminRequestStore.DeleteRequestor(existingRequestUser.Id);

                _context.SaveChanges();
                return RedirectToAction("Requests");
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
                return RedirectToAction("ServerError", "Accounts");
            }


        }





    }
}