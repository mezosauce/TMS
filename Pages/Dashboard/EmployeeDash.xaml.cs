using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel;

using Time_Managmeent_System.Services;
using Time_Management_System.Pages;
using Time_Managmeent_System.Models;

namespace Time_Managmeent_System.Pages.Dashboard;

public partial class EmployeeDash : ContentPage, INotifyPropertyChanged
{
    CancellationTokenSource _cts;
    bool _isTracking = false;
    private System.Timers.Timer _timer;

    private readonly DataService _dataService;

    private UserProfile _userProfile;
    public UserProfile UserProfile
    {
        get => _userProfile;
        set
        {
            if (_userProfile != value)
            {
                _userProfile = value;
                OnPropertyChanged(nameof(UserProfile));
                OnPropertyChanged(nameof(FullName)); // Notify UI to update FullName too
            }
        }
    }
    public string FullName => $"{UserProfile?.First} {UserProfile?.Last}".Trim();

    public EmployeeDash(DataService dataService)
    {
        InitializeComponent();
        StartEasternTimeClock();
        _dataService = dataService;
        BindingContext = this;
        LoadProfileAsync();
    }

    private async void LoadProfileAsync()
    {
        try
        {
            // Get currently authenticated user from Supabase Auth
            var user = _dataService.SupabaseClient.Auth.CurrentUser;
            if (user == null)
            {
                await DisplayAlert("Error", "No authenticated user found.", "OK");
                return;
            }

            // Query your "UserProfile" table using the user's ID
            var response = await _dataService.SupabaseClient
                .From<UserProfile>()
                .Where(x => x.Id == user.Id)
                .Get();

            // Get first user profile
            var profile = response.Models.FirstOrDefault();
            if (profile == null)
            {
                await DisplayAlert("Error", "Profile data not found.", "OK");
                return;
            }



            // Set the profile to the property
            UserProfile = new UserProfile
            {
                Id = profile.Id,
                First = profile.First,
                Last = profile.Last,
                AvatarUrl = profile.AvatarUrl
            };

            // Optionally update the label text if you also show full name somewhere
            MainThread.BeginInvokeOnMainThread(() =>
            {
                // If you want to show "First Last" together in one label, update label text here
                // For example:
                // NameLabel.Text = $"{UserProfile.First} {UserProfile.Last}";
            });
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to load profile: {ex.Message}", "OK");
        }
    }

    private async void OnProfileTapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new EditProfile(_dataService));
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

    /*
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
     */
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
   
    private async void HomeClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new EmployeeDash(_dataService));
    }

    private async void ReportsClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new CalendarPage(_dataService));

    }
    private async void ScheduleClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new CalendarPage(_dataService));
    }

    private void TimeOffClicked(object sender, EventArgs e)
    {

    }
}