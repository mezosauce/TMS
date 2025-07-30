using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel;

using Time_Managmeent_System.Services;
using Time_Management_System.Pages;
using System.Collections.ObjectModel;
using Time_Managmeent_System.Models;

namespace Time_Managmeent_System.Pages.Dashboard.DashboardServices;

public partial class NotificationLog : ContentPage
{
    private readonly DataService _dataService;

    public ObservableCollection<Notifications> NotificationMessages { get; set; } = new();

    public NotificationLog(DataService dataService)
    {
        InitializeComponent();
        _dataService = dataService;
        LoadNotificationsAsync();
    }

    private async void LoadNotificationsAsync()
    {
        try
        {
            var currentUser = _dataService.SupabaseClient.Auth.CurrentUser;
            var allNotifications = await _dataService.SupabaseClient
                .From<Notifications>()
                .Where(n => n.Receiver_ID == currentUser.Id)
                .Get();

            Console.WriteLine("✅ Successfully fetched notifications.");

            if (allNotifications != null && allNotifications.Models.Count > 0)
            {
                foreach (var notification in allNotifications.Models)
                {
                    NotificationMessages.Add(notification);
                }

                NotificationList.ItemsSource = NotificationMessages;
                Console.WriteLine($"📬 Loaded {NotificationMessages.Count} notification(s).");
            }
            else
            {
                Console.WriteLine("ℹ️ No notifications found in the database.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error fetching notifications: {ex.Message}");
            await DisplayAlert("Error", $"Failed to load notifications: {ex.Message}", "OK");
        }
    }

    public static bool IsTradeRequest(Notifications notification)
    {
        return !string.IsNullOrEmpty(notification.message) && notification.message.Contains("SHIFT TRADE REQUEST");
    }
    private async void OnConfirmClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is Notifications notification)
        {
            try
            {
                // 1. Get sender and receiver IDs
                string senderId = notification.Sender_ID;
                string receiverId = notification.Receiver_ID;

                // 2. Fetch both users' future shifts
                var allShifts = await _dataService.SupabaseClient
                    .From<Time>()
                    .Get();

                var senderShift = allShifts.Models
                    .FirstOrDefault(s => s.User_ID == senderId && s.Shift_date > DateTime.UtcNow.Date);

                var receiverShift = allShifts.Models
                    .FirstOrDefault(s => s.User_ID == receiverId && s.Shift_date > DateTime.UtcNow.Date);

                if (senderShift == null || receiverShift == null)
                {
                    await DisplayAlert("Error", "Shift information could not be found for one of the users.", "OK");
                    return;
                }

                // 3. Swap their Worker_IDs
                var tempId = senderShift.User_ID;
                senderShift.User_ID = receiverShift.User_ID;
                receiverShift.User_ID = tempId;

                // 4. Update the shifts in the database
                await _dataService.SupabaseClient
                    .From<Time>()
                    .Update(senderShift);

                await _dataService.SupabaseClient
                    .From<Time>()
                    .Update(receiverShift);

                // 5. Delete the notification
                await _dataService.SupabaseClient
                    .From<Notifications>()
                    .Where(n => n.Notification_ID == notification.Notification_ID)
                    .Delete();

                // 6. Send confirmation back to requester
                var confirmNotification = new Notifications
                {
                    Notification_ID = Guid.NewGuid().ToString(),
                    Sender_ID = receiverId,      // you're confirming
                    Receiver_ID = senderId,      // original requester
                    message = "✅ Your shift trade request was accepted.",
                    Send_Time = DateTime.UtcNow,
                    Receive_Time = DateTime.UtcNow
                };

                await _dataService.SupabaseClient
                    .From<Notifications>()
                    .Insert(confirmNotification);

                // 7. Show confirmation and refresh list
                await DisplayAlert("Confirmed", "You accepted the shift trade request.", "OK");

                NotificationMessages.Remove(notification);
                LoadNotificationsAsync(); // optional refresh
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
            }
        }
    }


    private async void OnCancelClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is Notifications notification)
        {
            // Optional: Notify sender it was declined (future enhancement)
            await DisplayAlert("Declined", "You declined the shift trade request.", "OK");

            try
            {
                // Delete from database
                await _dataService.SupabaseClient
                    .From<Notifications>()
                    .Where(n => n.Notification_ID == notification.Notification_ID)
                    .Delete();

                // Send a reply notification to the original sender
                var replyNotification = new Notifications
                {
                    Notification_ID = Guid.NewGuid().ToString(),
                    Sender_ID = notification.Receiver_ID,    // you're the one replying
                    Receiver_ID = notification.Sender_ID,    // original requester
                    message = $"❌ Your shift trade request was declined.",
                    Send_Time = DateTime.UtcNow,
                    Receive_Time = DateTime.UtcNow
                };

                await _dataService.SupabaseClient
                    .From<Notifications>()
                    .Insert(replyNotification);

                NotificationMessages.Clear(); // clear existing
                LoadNotificationsAsync();     // reload from DB

            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Exception during cancel: {ex.Message}", "OK");
            }
        }
    }



}
