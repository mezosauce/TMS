using Microsoft.Maui.Controls;

namespace Time_Managmeent_System.Pages
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        private async void OnEmployeeLoginClicked(object sender, EventArgs e)
        {
            // Navigate to Employee login or main page
            await Navigation.PushAsync(new MainPage());
        }

        private async void OnManagerLoginClicked(object sender, EventArgs e)
        {
            // Navigate to Manager login or main page
            await Navigation.PushAsync(new MainPage());
        }

        private async void OnAdminLoginClicked(object sender, EventArgs e)
        {
            // Navigate to Admin login or main page
            await Navigation.PushAsync(new MainPage());
        }
    }
}