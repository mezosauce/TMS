namespace Time_Managmeent_System.Pages.LoginType;
using Microsoft.Maui.Storage;
using Supabase;
using Supabase.Gotrue;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System;
using System.Collections.ObjectModel;
using Time_Managmeent_System;

public partial class Admin : ContentPage
{
	public Admin(Supabase.Client supabase)
	{
		InitializeComponent();
		_supabase = supabase;
	}

    private async void OnLoginClicked(object sender, EventArgs e)
	{
        // validate credentials and navigate to the Admin dashboard
        // login logic has not been tested yet
        // hold email and password
        string email = EmailEntry.Text;
        string passsword = PasswordEntry.Text;

        try
        {
            var session = await _supabase.Auth.SignIn(email: email, passsword: passsword);

            // store tokens
            await SecureStorage.SetAsync("access_token", session.AccessToken);
            await SecureStorage.SetAsync("refresh_token", session.RefreshToken);

            await DisplayAlert("Login", "Employee login successful!", "OK");
            // navigate to the main page after login
            await Navigation.PushAsync(new Dashboard.AdminDash());

        }
        catch (Exception ex)
        {
            {
                await DisplayAlert("Login unsuccessful :(", ex.Message, "OK");
            }

        }
    }
}