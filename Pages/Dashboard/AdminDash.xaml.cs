using Time_Managmeent_System.Services;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using Supabase;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Time_Management_System.Pages;

namespace Time_Managmeent_System.Pages.Dashboard;

public partial class AdminDash : ContentPage
{
    CancellationTokenSource? _cts;
    bool _isTracking = false;
    private System.Timers.Timer? _timer;

    private readonly DataService _dataService;
    public AdminDash(DataService dataService)
    {
        InitializeComponent();
        StartEasternTimeClock();
        _dataService = dataService;
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

    private async void OnReportPageClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new ReportPage(_dataService));
    }

    private async void OnGeoFencingPageClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new GeoFencingPage());
    }

    private async void OnEditAccount(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new EditAccount());
    }

    private async void HomeClicked(object sender, EventArgs e)
    {
        // Navigate to the home page (replace with your actual home page)
        await Navigation.PushAsync(new AdminDash(_dataService));
    }
}