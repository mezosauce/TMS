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
                    LocationLabel.Text = $"Location Lat, Long: {location.Latitude:F4}, {location.Longitude:F4}";
                }
                else
                {
                    LocationLabel.Text = "Location: Unavailable";
                }
                await Task.Delay(3000, token); // Update every 3 seconds
            }
            catch (TaskCanceledException)
            {
                // Gracefully handle cancellation
                break;
            }
            catch (Exception ex)
            {
                LocationLabel.Text = $"Error: {ex.Message}";
            }
        }
    }

    private async void OnWorkerScheduleClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new CalendarPage());
    }
}