﻿using System.ComponentModel;
using Time_Managmeent_System.Services;
using Time_Management_System.Pages;
using Time_Managmeent_System.Models;
using Time_Managmeent_System.Pages.Dashboard.DashboardServices;
using System.Collections.ObjectModel;


namespace Time_Managmeent_System.Pages.Dashboard;

public partial class ManagerDash : ContentPage, INotifyPropertyChanged
{

    public event PropertyChangedEventHandler PropertyChanged;

    void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    CancellationTokenSource? _cts;
    bool _isTracking = false;
    private System.Timers.Timer? _timer;

    private readonly DataService _dataService;

    private UserProfile _userProfile;
    private List<Time> _userTimeLogs = new();
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

    public ObservableCollection<string> ShiftOptions { get; set; }

    public ManagerDash(DataService dataService)
    {
        InitializeComponent();
        StartEasternTimeClock();
        _dataService = dataService;
        ShiftOptions = new ObservableCollection<string>
        {
            "First Shift",
            "Second Shift",
            "Third Shift"
        };
        BindingContext = this;
        LoadProfileAsync();
    }


    private async Task LoadProfileAsync()
    {
        try
        {
            // ✅ Get the currently signed-in user
            var session = _dataService.SupabaseClient.Auth.CurrentSession;
            if (session?.User == null)
            {
                System.Diagnostics.Debug.WriteLine("⚠️ No user is currently logged in.");
                await DisplayAlert("Error", "User session not found.", "OK");
                return;
            }

            var user = session.User;

            // 🗓️ Optional: define cutoff to limit time logs (e.g. last 30 days)
            var cutoffDate = DateTime.UtcNow.AddDays(-30);

            // ✅ Fetch logs for this user
             var allTimeLogs = await _dataService.SupabaseClient
                .From<Time>()
                .Where(t => t.User_ID == user.Id)
                .Order(t => t.Clocked_in, Supabase.Postgrest.Constants.Ordering.Descending)
                .Get();

            _userTimeLogs = allTimeLogs.Models
                .Where(t => t.Clocked_in.HasValue && t.Clocked_in.Value >= cutoffDate)
                .ToList();

           

            // 🔁 Debug logging
            System.Diagnostics.Debug.WriteLine($"✅ Loaded {_userTimeLogs.Count} shifts for user {user.Email}");

            // 🎯 Now update any UI based on the logs
            UpdateUpcomingShiftLabel(); // Make sure this method exists
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ LoadProfileAsync failed: {ex.Message}");
            await DisplayAlert("Error", "Failed to load profile.", "OK");
        }
    }

    private void UpdateUpcomingShiftLabel()
    {
        if (_userTimeLogs == null || !_userTimeLogs.Any())
        {
            UpcomingShiftLabel.Text = "No upcoming shifts.";
            return;
        }

        var today = DateTime.Today;

        // Filter upcoming or today shifts
        var upcomingShift = _userTimeLogs
            .Where(t => t.Shift_date >= today)
            .OrderBy(t => t.Shift_date)
            .FirstOrDefault();

        if (upcomingShift == null)
        {
            UpcomingShiftLabel.Text = "No upcoming shifts.";
        }
        else
        {
            var startTime = GetShiftStartTime(upcomingShift.Shift_type, upcomingShift.Shift_date);
            var endTime = GetShiftEndTime(upcomingShift.Shift_type, upcomingShift.Shift_date);

            if (upcomingShift.Shift_date == today)
            {
                UpcomingShiftLabel.Text = $"Today’s Shift: {upcomingShift.Shift_type} ({startTime:t} - {endTime:t})";
            }
            else
            {
                UpcomingShiftLabel.Text = $"Next Shift: {upcomingShift.Shift_type} on {upcomingShift.Shift_date:MMMM dd} ({startTime:t} - {endTime:t})";
            }
        }
    }

    private DateTime GetShiftStartTime(string shiftType, DateTime shiftDate)
    {
        return shiftType switch
        {
            "First Shift" => shiftDate.Date.AddHours(9),
            "Second Shift" => shiftDate.Date.AddHours(17),
            "Third Shift" => shiftDate.Date.AddHours(22),
            _ => throw new ArgumentException("Invalid shift type")
        };
    }
    private DateTime GetShiftEndTime(string shiftType, DateTime shiftDate)
    {
        return shiftType switch
        {
            "First Shift" => shiftDate.Date.AddHours(17),           // 5:00 PM
            "Second Shift" => shiftDate.Date.AddHours(22),          // 10:00 PM
            "Third Shift" => shiftDate.Date.AddDays(1).AddHours(3), // 3:00 AM next day
            _ => throw new ArgumentException("Invalid shift type")
        };
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
        Location userLocation;
        try
        {
            userLocation = await Geolocation.GetLastKnownLocationAsync()
                           ?? await Geolocation.GetLocationAsync(new GeolocationRequest(GeolocationAccuracy.Medium));
        }
        catch (Exception ex)
        {
            await DisplayAlert("Location Error", $"Could not retrieve location: {ex.Message}", "OK");
            return;
        }

        if (userLocation == null)
        {
            await DisplayAlert("Location Error", "Unable to get location.", "OK");
            return;
        }

        // Debug log for user location
        System.Diagnostics.Debug.WriteLine($"📍 User Location: {userLocation.Latitude}, {userLocation.Longitude}");

        // Pull all active geo locations
        var allowedLocations = await _dataService.SupabaseClient
            .From<Geolocating>()
            .Where(g => g.Active == true)
            .Get();

        bool isInRange = false;

        foreach (var loc in allowedLocations.Models)
        {
            if (double.TryParse(loc.Latitude, out double lat) &&
                double.TryParse(loc.Longitude, out double lon))
            {
                double distance = HaversineDistance(
                    userLocation.Latitude,
                    userLocation.Longitude,
                    lat,
                    lon
                );

                // Debug log for each location check
                System.Diagnostics.Debug.WriteLine($"📍 Allowed: {loc.Name} at ({lat}, {lon}) -> Distance: {distance} meters");

                if (distance <= 1609.34) // 1 mile == 1609.34 meters
                                         // 5 miles is == 8046.72 meters
                {
                    isInRange = true;
                    break;
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"⚠️ Failed to parse lat/lon: {loc.Latitude}, {loc.Longitude}");
            }
        }

        if (!isInRange)
        {
            await DisplayAlert("Access Denied", "You must be within 1 mile of an authorized location to clock in or out.", "OK");
            return;
        }

        // --- ⏱ Clock In/Out Logic ---
        var user = _dataService.SupabaseClient.Auth.CurrentUser;
        if (user == null)
        {
            await DisplayAlert("Auth Error", "User not logged in.", "OK");
            return;
        }
        var userId = user.Id;
        var today = DateTime.Today;
        var now = DateTimeOffset.UtcNow;
        var shiftType = GetShiftType(now.TimeOfDay);

        if (ClockButton.Text == "Clock In")
        {
            var newEntry = new Time
            {
                Tid = Guid.NewGuid().ToString(),
                User_ID = user.Id,
                Clocked_in = now,
                Clocked_out = DateTimeOffset.MinValue,
                Hours = 0,
                Status = false,
                Shift_date = today,
                Shift_type = shiftType
            };

            await _dataService.SupabaseClient
                .From<Time>()
                .Insert(newEntry);

            await DisplayAlert("Clock", "Clock-in successful!", "OK");
            ClockButton.Text = "Clock Out";
        }
        else if (ClockButton.Text == "Clock Out")
        {


            System.Diagnostics.Debug.WriteLine($"DEBUG: today = {today.ToShortDateString()}");

            var result = await _dataService.SupabaseClient
                .From<Time>()
                .Where(t => t.User_ID == user.Id && t.Shift_date == today)
                .Get();

            System.Diagnostics.Debug.WriteLine($"DEBUG: Retrieved {result.Models.Count} time logs.");

            var existingShift = result.Models.FirstOrDefault();
            if (existingShift != null)
            {
                existingShift.Clocked_out = now;
                if (existingShift.Clocked_in.HasValue)
                {
                    existingShift.Hours = (now - existingShift.Clocked_in.Value).TotalHours;
                }
                existingShift.Status = true;

                await _dataService.SupabaseClient
                    .From<Time>()
                    .Update(existingShift);

                await DisplayAlert("Clock", $"Successfully Clocked-Out. Total: {existingShift.Hours:F2} hrs", "OK");
                ClockButton.Text = "Clock In";
            }
            else
            {
                await DisplayAlert("Error", "No open shift to clock out from.", "OK");
            }
        }
    }

    private string GetShiftType(TimeSpan now)
    {
        if (now >= new TimeSpan(9, 0, 0) && now < new TimeSpan(17, 0, 0))
            return "First Shift";
        if (now >= new TimeSpan(17, 0, 0) && now < new TimeSpan(22, 0, 0))
            return "Second Shift";
        return "Third Shift";
    }




    private double HaversineDistance(double lat1, double lon1, double lat2, double lon2)
    {
        const double earthRadius = 6371000; // in meters

        double dLat = DegreesToRadians(lat2 - lat1);
        double dLon = DegreesToRadians(lon2 - lon1);

        double radLat1 = DegreesToRadians(lat1);
        double radLat2 = DegreesToRadians(lat2);

        double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                   Math.Cos(radLat1) * Math.Cos(radLat2) *
                   Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return earthRadius * c;
    }

    private double DegreesToRadians(double degrees)
    {
        return degrees * Math.PI / 180;
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
        await Navigation.PushAsync(new ReportPage(_dataService));
    }

    private async void EditEmployeesClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new EditAccount(_dataService));
    }
    private async void EditScheduling(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new AssignShiftPage(_dataService));
    }
    private async void OnBellTapped(object sender, EventArgs e)
    {

        await Navigation.PushAsync(new NotificationLog(_dataService));
    }
}