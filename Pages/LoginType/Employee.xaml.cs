namespace Time_Managmeent_System.Pages.LoginType;
using Microsoft.Maui.Storage;
using Supabase;
using Supabase.Gotrue;
using Time_Managmeent_System;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System.Collections.ObjectModel;

public partial class Employee : ContentPage
{

    public Employee()
    {
        InitializeComponent();
        
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        // validate credentials and navigate to the employee dashboard
        // login logic has not been tested yet
        // hold email and password

        /*
        string email = EmailEntry.Text;
        string passsword = PasswordEntry.Text;

        try
        {
            var session = await _supabase.Auth.SignIn(email: email, password: passsword);

            // store tokens
            await SecureStorage.SetAsync("access_token", session.AccessToken);
            await SecureStorage.SetAsync("refresh_token", session.RefreshToken);

            await DisplayAlert("Login", "Employee login successful!", "OK");
            // navigate to the main page after login
            await Navigation.PushAsync(new Dashboard.EmployeeDash());

        }
        catch (Exception ex)
        {
            {
                await DisplayAlert("Login unsuccessful :(", ex.Message, "OK");
            }

        }
    }
        */
        await DisplayAlert("Login", "Employee login successful!", "OK");
        await Navigation.PushAsync(new Dashboard.EmployeeDash()); // Navigate to the main page after login
    }
    }