
using FruityNET.Entities;

using Microsoft.AspNetCore.Identity;
using System.Linq;

namespace FruityNET.Data
{

    public class NotificationCount
    {
        private readonly ApplicationDbContext _Context;
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        public int count { get; set; }


        public NotificationCount(ApplicationDbContext _Context, UserManager<User> userManager,
        SignInManager<User> signInManager)
        {
            this._Context = _Context;
            this.userManager = userManager;
            this.signInManager = signInManager;

        }

        public int GetCount()
        {
            var _currentUser = _Context.Users.Find(userManager.GetUserId(System.Security.Claims.ClaimsPrincipal.Current));
            count = _Context.Notification.ToList().FindAll(x => x.RecieverUsername == _currentUser.UserName).Count;
            return count;
        }

    }

    public class RequestNotificationCount
    {
        public int Count { get; set; }
    }
}