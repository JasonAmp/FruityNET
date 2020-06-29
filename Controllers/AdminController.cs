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
         INotificationBox _notificationBox, ILogger<AccountsController> _logger)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this._context = _context;
            this._AdminRequestStore = _AdminRequestStore;
            this._logger = _logger;
            this._notificationBox = _notificationBox;
        }


        [HttpGet]
        public IActionResult Requests()
        {
            try
            {
                var CurrentUser = _context.Users.Find(userManager.GetUserId(User));
                if (CurrentUser is null)
                    throw new DomainException(ErrorMessages.NotSignedIn);

                var AdminRequests = _AdminRequestStore.GetAll();
                var AdminRequestModel = new AdminRequestsViewModel() { };
                foreach (var Request in AdminRequests)
                {
                    AdminRequestModel.AdminRequests.Add(new AdminRequestDTO()
                    {
                        RequestDate = Request.RequestDate,
                        RequestId = Request.Id,
                        Username = Request.Username,

                    });
                }

                return View(AdminRequestModel);

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
            var Request = new AdminRequest()
            {
                Username = CurrentUser.UserName,
                RequestDate = DateTime.Now
            };

            return View();
        }





    }
}