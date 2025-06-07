using Microsoft.Maui.Controls;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System.Collections.ObjectModel;
using Time_Managmeent_System;
using static System.Net.WebRequestMethods;




namespace Time_Managmeent_System.Pages
{

    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();

            var SUPABASE_URL = EnvironmentConfig.SUPABASE_URL;
            var SUPABASE_KEY = EnvironmentConfig.SUPABASE_KEY;

            var supabaseClient = new Supabase.Client(SUPABASE_URL, SUPABASE_KEY);
            _ = supabaseClient.InitializeAsync();
        }

        public ObservableCollection<Employee> Users { get; set; } = new();

        private readonly Supabase.Client _supabase;

        public LoginPage(Supabase.Client supabase)
        {
            _supabase = supabase;
            _ = LoadUsersAsync();
        }


        public class Employee : BaseModel
        {
            [PrimaryKey("id", false)]
            public int Id { get; set; }

            [Column("Username")]
            public string Username { get; set; }

            [Column("Password")]
            public string Password { get; set; }

            [Column("Position")]
            public string Position { get; set; }

            [Column("First")]
            public string First { get; set; }

            [Column("Last")]
            public string Last { get; set; }
        }

        private async Task LoadUsersAsync()
        {
            var result = await _supabase.From<Employee>().Get();

            if (result.Models is IEnumerable<Employee> users)
            {
                foreach (var user in users)
                    Users.Add(user);
            }
        }

        private async void OnEmployeeLoginClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new LoginType.Employee());
        }

        private async void OnManagerLoginClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new LoginType.Manager());
        }

        private async void OnAdminLoginClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new LoginType.Admin());
        }

        private async void OnRegisterClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new LoginType.Guest());
        }

        private void OnToggleDarkModeClicked(object sender, EventArgs e)
        {
            if (App.Current.UserAppTheme == AppTheme.Dark)
            {
                App.Current.UserAppTheme = AppTheme.Light;
                if (sender is Button btn)
                {
                    btn.Text = "\uf185";
                    btn.TextColor = Colors.Black;
                }
            }
            else
            {
                App.Current.UserAppTheme = AppTheme.Dark;
                if (sender is Button btn)
                {
                    btn.Text = "\uf186";
                    btn.TextColor = Colors.White;
                }
            }
        }
    }
 
}