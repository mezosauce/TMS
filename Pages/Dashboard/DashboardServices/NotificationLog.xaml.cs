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
            var allNotifications = await _dataService.SupabaseClient
                .From<Notifications>()
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

}
