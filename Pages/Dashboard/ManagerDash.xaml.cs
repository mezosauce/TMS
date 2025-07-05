using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Time_Managmeent_System.Services;
using Time_Management_System.Pages;


namespace Time_Managmeent_System.Pages.Dashboard;

public partial class ManagerDash : ContentPage
{
    CancellationTokenSource? _cts;
    bool _isTracking = false;
    private System.Timers.Timer? _timer;

    private readonly DataService _dataService;
    public ManagerDash(DataService dataService)
    {
        InitializeComponent();
        StartEasternTimeClock();
        _dataService = dataService;
    }

    private void StartEasternTimeClock()
    {
        _timer = new System.Timers.Timer(1000);
        _timer.Elapsed += UpdateEasternTimeClock;
        _timer.AutoReset = true;
        _timer.Start();
    }

    private void UpdateEasternTimeClock(object? sender, System.Timers.ElapsedEventArgs e)
    {
        var easternTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
        var easternTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, easternTimeZone);

        MainThread.BeginInvokeOnMainThread(() =>
        {
            EasternTimeLabel.Text = easternTime.ToString("hh:mm:ss tt");
        });
    }

    private async void OnClockClicked(object sender, EventArgs e)
    {
        //CLOCK IN AND OUT LOGIC
        if (ClockButton.Text == "Clock Out")
        {
            await DisplayAlert("Clock", "Successfully Clocked-Out", "OK");
            ClockButton.Text = "Clock In"; // Change button text to "Clock In"
            return; // Exit the method if already clocked out
        }
        await DisplayAlert("Clock", "Clock-in successful!", "OK");
        ClockButton.Text = "Clock Out"; // Change button text to "Clock Out"
    }

    private async void HomeClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new ManagerDash(_dataService));
    }

    private async void OnProfileTapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new EditProfile(_dataService));
    }

    private async void ScheduleClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new CalendarPage(_dataService));
    }

    private async void ReportsClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new CalendarPage(_dataService));
    }

    private async void EditEmployeesClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new EditAccount(_dataService));
    }
}