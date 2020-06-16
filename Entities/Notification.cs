using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FruityNET.Entities
{
    public class Notification
    {
        [Key]
        public Guid Id { get; set; }

        public string Message { get; set; }

        public Guid NotificationBoxId { get; set; }

        public DateTime NotificationDate { get; set; }

        [ForeignKey("NotificationBoxId")]
        public NotificationBox NotificationBox { get; set; }
        public string RecieverUsername { get; set; }
    }

}