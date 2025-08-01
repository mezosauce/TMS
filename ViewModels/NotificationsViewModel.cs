using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Time_Managmeent_System.Models;

namespace Time_Managmeent_System.ViewModels;

public class NotificationViewModel
{
    public Notifications Notification { get; set; }

    public bool IsShiftTradeRequest =>
        !string.IsNullOrEmpty(Notification.message) &&
        Notification.message.StartsWith("SHIFT TRADE REQUEST");

    public bool IsGeneralMessage => !IsShiftTradeRequest;

    // Optional helper properties
    public string Message => Notification.message;
    public string SenderId => Notification.Sender_ID;
    public string ReceiverId => Notification.Receiver_ID;

    public NotificationViewModel(Notifications notification)
    {
        Notification = notification;
    }
}
