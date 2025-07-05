using Time_Managmeent_System.Services;
using Time_Management_System.Pages;
using Time_Managmeent_System.Models;


namespace Time_Managmeent_System.Pages.Dashboard;

public partial class AdminDash : ContentPage
{
    CancellationTokenSource? _cts;
    bool _isTracking = false;
    private System.Timers.Timer? _timer;

    private readonly DataService _dataService;
    public UserProfile _userProfile { get; set; }

    public AdminDash(DataService dataService)
    {
        InitializeComponent();
        StartEasternTimeClock();
        _dataService = dataService;
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
        await Navigation.PushAsync(new EditProfile(_dataService));
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
        await Navigation.PushAsync(new CalendarPage(_dataService));
    }

    private async void OnReportPageClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new ReportPage(_dataService));
    }

    private async void OnGeoFencingPageClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new GeoFencingPage(_dataService));
    }

    private async void OnEditAccount(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new EditAccount(_dataService));
    }

    private async void HomeClicked(object sender, EventArgs e)
    {
        // Navigate to the home page (replace with your actual home page)
        await Navigation.PushAsync(new AdminDash(_dataService));
    }
}