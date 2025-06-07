using Microsoft.Maui.Controls;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System.Collections.ObjectModel;
using Time_Managmeent_System;
using static System.Net.WebRequestMethods;
using Supabase;

namespace Time_Managmeent_System.Pages
{

    public partial class LoginPage : ContentPage
    {
        private readonly Supabase.Client _supabase;

        public LoginPage()
        {
            InitializeComponent();
           
        }


        public ObservableCollection<Employee> Users { get; set; } = new();


        [Table("Employee")]
        public class Employee : BaseModel
        {

            [PrimaryKey("id", false)]
            public  int Id { get; set; }
            [Column("Username")]
            public  string Username { get; set; }
            [Column("Password")]
            public  string Password { get; set; }
            [Column("Position")]
            public  string Position { get; set; }
            [Column("First")]
            public  string First { get; set; }
            [Column("Last")]
            public  string Last { get; set; }
        }

        private async Task<List<Employee>> GetEmployeesAsync()
        {

            var response = await _supabase.From("employee").Get();

            if (response.Status == 200)
            {

                return response.Models.ToList();
            }

            else
            {
                throw new Exception(response.Error.Message);
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