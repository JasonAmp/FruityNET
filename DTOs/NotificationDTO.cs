using System;
using System.Collections.Generic;

namespace FruityNET.DTOs
{
    public class NotificationsViewDTO
    {
        public List<NotificationDTO> AllNotifications { get; set; }
        public NotificationsViewDTO()
        {
            AllNotifications = new List<NotificationDTO>();
        }
    }

    public class NotificationDTO
    {

        public Guid Id { get; set; }

        public string Message { get; set; }

        public Guid NotificationBoxId { get; set; }

        public string RecieverUsername { get; set; }

        public double ElapsedHour { get; set; }
        public double ElapsedMinute { get; set; }

    }
}