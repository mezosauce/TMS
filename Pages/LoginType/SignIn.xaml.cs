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

   
    private async void OnLoginClicked(object sender, EventArgs e)
    {
        // await DisplayAlert("Login", "Manager login successful!", "OK");
        //  await Navigation.PushAsync(new Dashboard.ManagerDash()); // Navigate to the main page after login
        // validate credentials and navigate to the manager dashboard
        // login logic has not been tested yet
        // hold email and password
        // commented out until it can be properly tested


        
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
                await Navigation.PushAsync(new Dashboard.ManagerDash()); 
            }
            else
            {
                await DisplayAlert("Login Failed", "Invalid email or password.", "OK");
                return;
            }
                // store tokens
              /*  await SecureStorage.SetAsync("access_token", session.AccessToken);
            await SecureStorage.SetAsync("refresh_token", session.RefreshToken);

            await DisplayAlert("Login", "Employee login successful!", "OK");
            // navigate to the main page after login
            await Navigation.PushAsync(new Dashboard.ManagerDash());
            */

        }
        catch (Exception ex)
        {
            {
                await DisplayAlert("Login unsuccessful :(", ex.Message, "OK");
            }

        }
        
        
    }
}