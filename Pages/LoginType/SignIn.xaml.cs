using Time_Managmeent_System.Services;
using Supabase;
namespace Time_Managmeent_System.Pages.LoginType;

public partial class SignIn : ContentPage
{
    private readonly Client _supabase;

    public SignIn(DataService dataService)
    {
        InitializeComponent();
        _supabase = dataService.SupabaseClient; // Get the Supabase client from the data service
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

        try
        {
            var session = await _supabase.Auth.SignIn(email: email, password: password);

            // store tokens
            await SecureStorage.SetAsync("access_token", session.AccessToken);
            await SecureStorage.SetAsync("refresh_token", session.RefreshToken);

            await DisplayAlert("Login", "Employee login successful!", "OK");
            // navigate to the main page after login
            await Navigation.PushAsync(new Dashboard.ManagerDash());

        }
        catch (Exception ex)
        {
            {
                await DisplayAlert("Login unsuccessful :(", ex.Message, "OK");
            }

        }
        
        
    }
}