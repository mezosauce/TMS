using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace Time_Managmeent_System.Pages.Dashboard;

public partial class EmployeeDash : ContentPage
{
    CancellationTokenSource _cts;
    bool _isTracking = false;
    private System.Timers.Timer _timer;

    public EmployeeDash()
    {
        InitializeComponent();
        StartEasternTimeClock();
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

    async void OnTrackLocationClicked(object sender, EventArgs e)
    {
        if (!_isTracking)
        {
            _isTracking = true;
            TrackLocationButton.Text = "Stop Tracking";
            _cts = new CancellationTokenSource();
            await StartTrackingAsync(_cts.Token);
        }
        else
        {
            _isTracking = false;
            TrackLocationButton.Text = "Start Tracking";
            _cts?.Cancel();
        }
    }

    async Task StartTrackingAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            try
            {
                var location = await Geolocation.GetLastKnownLocationAsync();
                if (location != null)
                {
                    LocationLabel.Text = $"Location: {location.Latitude:F4}, {location.Longitude:F4}";
                }
                else
                {
                    LocationLabel.Text = "Location: Unavailable";
                }
            }
            catch (Exception ex)
            {
                LocationLabel.Text = $"Error: {ex.Message}";
            }
            await Task.Delay(3000, token); // Update every 3 seconds
        }
    }

    private void StartEasternTimeClock()
    {
        _timer = new System.Timers.Timer(1000);
        _timer.Elapsed += (s, e) =>
        {
            var easternZone = TimeZoneInfo.FindSystemTimeZoneById(
#if WINDOWS
                "Eastern Standard Time"
#else
                "America/New_York"
#endif
            );
            var easternTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, easternZone);
            MainThread.BeginInvokeOnMainThread(() =>
            {
                EasternTimeLabel.Text = $" {easternTime:hh:mm:ss tt}";
            });
        };
        _timer.Start();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _timer?.Stop();
        _timer?.Dispose();
    }
}