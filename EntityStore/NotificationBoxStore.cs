using System;
using System.Collections.Generic;
using FruityNET.Data;
using FruityNET.Entities;
using FruityNET.IEntityStore;
using System.Linq;

namespace FruityNET.EntityStore
{
    public class NotificationBoxStore : INotificationBox
    {
        private ApplicationDbContext _Context;
        public NotificationBoxStore(ApplicationDbContext _Context)
        {
            this._Context = _Context;
        }
        public NotificationBox CreateNotificationBox(NotificationBox NotificationBox)
        {
            _Context.NotificationBox.Add(NotificationBox);
            _Context.SaveChanges();
            return NotificationBox;
        }

        public Notification DeleteNotifcation(Notification Notification)
        {
            _Context.Notification.Remove(Notification);
            _Context.SaveChanges();
            return Notification;
        }

        public Notification GetNotificationById(Guid Id)
        {
            return _Context.Notification.Find(Id);
        }

        public NotificationBox GetNotificationBoxByUserId(string Id)
        {
            return _Context.NotificationBox.FirstOrDefault(x => x.UserId == Id);
        }

        public List<Notification> GetUserNotifications(string Username)
        {
            var Query = from x in _Context.Notification
                        where x.RecieverUsername == Username
                        select x;
            return Query.ToList();
        }

        public Notification SendNotifcation(Notification notification)
        {
            _Context.Notification.Add(notification);
            return notification;
        }
    }
}