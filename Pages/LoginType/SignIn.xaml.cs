using Time_Managmeent_System.Services;
using Supabase;

namespace Time_Managmeent_System.Pages.LoginType;

public partial class SignIn : ContentPage
{
    private readonly DataService _dataService;

    public SignIn(DataService dataService)
    {
        InitializeComponent();
        _dataService = dataService;
    }


    public async Task<UserProfile> GetUserProfile(string userId)
    {
        var response = await _dataService.SupabaseClient
            .From("User_Data") // Use underscores instead of spaces
            .Select<UserProfile>("*")
            .Eq("id", userId)
            .SingleAsync(); // Use SingleAsync for asynchronous operation

        return response;
    }

    // Define a UserProfile class to match the structure of your User Data table
    public class UserProfile
    {
        public int Id { get; set; } 
        public string First { get; set; }
        public string Last { get; set; }
        public string Position { get; set; }
        public string AvatarUrl { get; set; }
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        string email = EmailEntry.Text;
        string password = PasswordEntry.Text;

        // Input validation
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            await DisplayAlert("Input Error", "Please enter both email and password.", "OK");
            return;
        }
        try
        {
            var session = await _dataService.SupabaseClient.Auth.SignIn(email, password);
            if (session.User != null)
            {
                await DisplayAlert("SUCCESS", "Login successful!", "OK");

                // Retrieve user profile information
                var userId = session.User.Id; // Assuming you have the user ID
                var userProfile = await GetUserProfile(userId); // Fetch user data

                if (userProfile != null)
                {
                    // Check the Position attribute
                    switch (userProfile.Position)
                    {
                        case "Manager":
                            await Navigation.PushAsync(new Dashboard.ManagerDash());
                            break;
                        case "Employee":
                            await Navigation.PushAsync(new Dashboard.EmployeeDash());
                            break;
                        // Add more cases as needed for different positions
                        default:
                            await DisplayAlert("Position Error", "Unknown position. Cannot navigate.", "OK");
                            break;
                    }
                }
                else
                {
                    await DisplayAlert("Profile Error", "Unable to retrieve user profile.", "OK");
                }
            }
            else
            {
                await DisplayAlert("Login Failed", "Invalid email or password.", "OK");
                return;
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Login unsuccessful :(", ex.Message, "OK");
        }
    }
}