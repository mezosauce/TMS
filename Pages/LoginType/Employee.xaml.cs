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

        var SUPABASE_URL = Environment.GetEnvironmentVariable("SUPABASE_URL");
        var SUPABASE_KEY = Environment.GetEnvironmentVariable("SUPABASE_KEY");

        var supabaseClient = new Supabase.Client(SUPABASE_URL, SUPABASE_KEY);
        _ = supabaseClient.InitializeAsync();
    }



    private async void OnLoginClicked(object sender, EventArgs e)
    {
        // validate credentials and navigate to the admin dashboard
        // login logic has not been tested yet
        // hold email and password
        await DisplayAlert("Login", "Employee login successful!", "OK");
    }
}