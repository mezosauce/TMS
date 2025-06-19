namespace Time_Managmeent_System.Pages.LoginType;
using Microsoft.Maui.Storage;
using Supabase;
using Time_Managmeent_System.Services;

public partial class Employee : ContentPage
{
    //private readonly Client _supabaseClient;

    /* private Client GetSupabaseClientFromDataService(DataService dataService)
     {
         // Use reflection to access the private _supabaseClient field
         var field = typeof(DataService).GetField("_supabaseClient", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
         return (Client)field.GetValue(dataService);
     }*/
    public Employee()
    {
        InitializeComponent();
        //_supabaseClient = GetSupabaseClientFromDataService(dataService);
    }


    private async void OnLoginClicked(object sender, EventArgs e)
    {

        /*string email = EmailEntry.Text;
        string password = PasswordEntry.Text;

        try
        {
            var session = await _supabaseClient.Auth.SignIn(email: email, password: password);

            // store tokens
            await SecureStorage.SetAsync("access_token", session.AccessToken);
            await SecureStorage.SetAsync("refresh_token", session.RefreshToken);

            await DisplayAlert("Login", "Employee login successful!", "OK");
            // navigate to the main page after login
            await Navigation.PushAsync(new Dashboard.EmployeeDash());
        }
        catch (Exception ex)
        {
            await DisplayAlert("Login unsuccessful :(", ex.Message, "OK");
        }
    */
        await DisplayAlert("Login", "Employee login successful!", "OK");
        // navigate to the main page after login
        await Navigation.PushAsync(new Dashboard.EmployeeDash());
    }


}