using System;
using FruityNET.DTOs;
using FruityNET.Entities;
using FruityNET.IEntityStore;
using Microsoft.AspNetCore.Identity;
using System.Linq;

namespace FruityNET.Queries
{
    public class GetAllNotificationsQuery
    {
        private readonly IUserStore _UserStore;
        private readonly INotificationBox _NotificationBox;
        private readonly UserAccount _CurrentAccount;
        private readonly IdentityUser _CurrentUser;

        public GetAllNotificationsQuery(IUserStore _UserStore, INotificationBox _NotificationBox,
        UserAccount _CurrentAccount, IdentityUser _CurrentUser)
        {
            this._CurrentUser = _CurrentUser;
            this._CurrentAccount = _CurrentAccount;
            this._NotificationBox = _NotificationBox;
            this._UserStore = _UserStore;
        }

        public NotificationsViewDTO Handle()
        {
            var NotificationsViewDTO = new NotificationsViewDTO() { };
            var Notifications = from x in _NotificationBox.GetUserNotifications(_CurrentUser.UserName)
                                orderby x.NotificationDate descending
                                select x;

            foreach (var notification in Notifications)
            {
                var ElapsedMinutes = DateTime.Now.Subtract(notification.NotificationDate).TotalMinutes;
                var ElapsedHours = DateTime.Now.Subtract(notification.NotificationDate).TotalHours;
                var ElapsedDays = DateTime.Now.Subtract(notification.NotificationDate).TotalDays;

                var ElapsedMonths = DateTime.Now.Subtract(notification.NotificationDate).TotalDays / 12;
                NotificationsViewDTO.AllNotifications.Add(new NotificationDTO()
                {
                    Id = notification.Id,
                    Message = notification.Message,
                    NotificationBoxId = notification.NotificationBoxId,
                    RecieverUsername = notification.RecieverUsername,
                    ElapsedHour = Math.Round(ElapsedHours),
                    ElapsedMinute = Math.Round(ElapsedMinutes),
                    ElapsedDay = Math.Round(ElapsedDays)
                });
            }
            return NotificationsViewDTO;
        }



    }
}