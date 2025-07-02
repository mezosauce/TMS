using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Time_Management_System.Pages;

namespace Time_Managmeent_System.Pages.Dashboard;

public partial class AdminDash : ContentPage
{
    CancellationTokenSource? _cts;
    bool _isTracking = false;
    private System.Timers.Timer? _timer;

    public AdminDash()
    {
        InitializeComponent();
        StartEasternTimeClock();
    }

    private void StartEasternTimeClock()
    {
        // Set the timer to update every second
        _timer = new System.Timers.Timer(1000);
        _timer.Elapsed += UpdateEasternTimeClock;
        _timer.AutoReset = true;
        _timer.Start();
    }
    private async void OnProfileTapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new EditProfile());
    }

    private void UpdateEasternTimeClock(object? sender, System.Timers.ElapsedEventArgs e)
    {
        // Get the current Eastern Time
        var easternTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
        var easternTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, easternTimeZone);

        // Update the label on the UI thread
        MainThread.BeginInvokeOnMainThread(() =>
        {
            EasternTimeLabel.Text = easternTime.ToString("hh:mm:ss tt");
        });
    }


    private async void OnWorkerScheduleClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new CalendarPage());
    }

    private void Button_Clicked(object sender, EventArgs e)
    {

    }
    private async void SetGeoLocation(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new SetGeoLocation());
    }

    private async void OnEditAccount(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new EditAccount());
    }
}