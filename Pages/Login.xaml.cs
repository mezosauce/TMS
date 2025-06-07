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
        private readonly Supabase.Client _supabase;

        public LoginPage()
        {
            InitializeComponent();
            BindingContext = this;

            var SUPABASE_URL = EnvironmentConfig.SUPABASE_URL;
            var SUPABASE_KEY = EnvironmentConfig.SUPABASE_KEY;

            _supabase = new Supabase.Client(SUPABASE_URL, SUPABASE_KEY);
            _ = _supabase.InitializeAsync();
            _ = LoadUsersAsync();

            Users.Add(new Employee { Username = "test", Position = "Tester", First = "Test", Last = "User" });
        }

        public ObservableCollection<Employee> Users { get; set; } = new();

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
            try
            {
                var result = await _supabase.From<Employee>().Get();
                System.Diagnostics.Debug.WriteLine($"Result: {result}");
                System.Diagnostics.Debug.WriteLine($"Models: {result.Models}");

                if (result.Models is IEnumerable<Employee> users)
                {
                    foreach (var user in users)
                        Users.Add(user);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading users: {ex}");
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