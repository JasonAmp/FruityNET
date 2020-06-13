using System;
using System.Collections.Generic;
using FruityNET.Entities;

namespace FruityNET.IEntityStore
{
    public interface INotificationBox
    {
        NotificationBox CreateNotificationBox(NotificationBox notificationBox);
        Notification SendNotifcation(Notification notification);
        Notification DeleteNotifcation(Notification notification);
        List<Notification> GetUserNotifications(string Username);
        Notification GetNotificationById(Guid Id);
        NotificationBox GetNotificationBoxByUserId(string Id);
    }
}